#nullable enable
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NetRabbit.Extensions.Builders;
using NetRabbit.Extensions.Models;

namespace NetRabbit.Extensions
{
    [Generator]
#pragma warning disable CA1812
    internal class SourceGenerator : IIncrementalGenerator
#pragma warning restore CA1812
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var invocations = context.SyntaxProvider.CreateSyntaxProvider
            (
                predicate: static (s, _) => IsSyntaxTargetForGeneration(s),
                transform: static (ctx, _) => (InvocationExpressionSyntax)ctx.Node
            ).Where(static m => m is not null);

            var compilationAndClasses = context.CompilationProvider.Combine(invocations.Collect());

            context.RegisterSourceOutput
            (
                compilationAndClasses,
                static (spc, source) => Execute(source.Left, source.Right, spc)
            );
        }

        private static bool IsSyntaxTargetForGeneration(SyntaxNode node)
        {
            return node is InvocationExpressionSyntax
            {
                Expression: MemberAccessExpressionSyntax { Name.Identifier.ValueText: "AddRabbit" }
            };
        }

        private static void Execute(Compilation compilation, ImmutableArray<InvocationExpressionSyntax> targets, SourceProductionContext context)
        {
            if (targets.IsDefaultOrEmpty)
            {
                return;
            }

            Execute(compilation, context);
        }

        private static void Execute(Compilation compilation, SourceProductionContext context)
        {
            var handlers = new List<MessageHandlerInfo>();

            foreach (var namespaceOrTypeSymbol in compilation.GlobalNamespace.GetMembers())
            {
                FlattenNamespaces(namespaceOrTypeSymbol, handlers);
            }

            if (handlers.Count == 0)
            {
                return;
            }

            var iocExtensions = FormatCode(ExtensionsBuilder.Build(handlers));
            context.AddSource("RabbitServicesExtensions.g.cs", iocExtensions);
        }

        private static void FlattenNamespaces
        (
            ISymbol namespaceOrTypeSymbol,
            List<MessageHandlerInfo> handlers
        )
        {
            if (namespaceOrTypeSymbol.Name is "System" or "Microsoft" or "<Module>") return;

            switch (namespaceOrTypeSymbol)
            {
                case INamespaceSymbol nsSymbol:
                    foreach (var x in nsSymbol.GetMembers())
                    {
                        FlattenNamespaces(x, handlers);
                    }
                    break;
                case ITypeSymbol tSymbol:

                    var classNameSpace = tSymbol.ContainingNamespace.ToDisplayString();

                    if (classNameSpace.StartsWith("NetRabbit.", StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }

                    if (tSymbol.IsAbstract)
                    {
                        break;
                    }

                    if (tSymbol is INamedTypeSymbol { IsGenericType: true })
                    {
                        break;
                    }

                    if (tSymbol.BaseType?.Name is "SynchronizedMessageHandler")
                    {
                        var baseType = tSymbol.BaseType;

                        (string?, string?) genericArg = default;

                        if (baseType.TypeArguments.Length == 1)
                        {
                            genericArg = (baseType.TypeArguments[0].ToDisplayString(), baseType.TypeArguments[0].ContainingNamespace.ToDisplayString());
                        }

                        handlers.Add(new MessageHandlerInfo
                        {
                            GenericArg = genericArg,
                            Handler = (classNameSpace, tSymbol.Name),
                            IsSyncHandler = true
                        });
                    }

                    foreach (var @interface in tSymbol.AllInterfaces)
                    {
                        if (@interface.Name is not "IMessageHandlerAsync")
                        {
                            continue;
                        }

                        (string?, string?) genericArg = default;

                        if (@interface.TypeArguments.Length == 1)
                        {
                            genericArg = (@interface.TypeArguments[0].ToDisplayString(), @interface.TypeArguments[0].ContainingNamespace.ToDisplayString());
                        }

                        handlers.Add(new MessageHandlerInfo
                        {
                            GenericArg = genericArg,
                            Handler = (classNameSpace, tSymbol.Name),
                            IsSyncHandler = false
                        });
                    }

                    break;
            }
        }

        private static string FormatCode(string source)
        {
            return SyntaxFactory.ParseCompilationUnit(source).NormalizeWhitespace().ToString();
        }
    }
}
