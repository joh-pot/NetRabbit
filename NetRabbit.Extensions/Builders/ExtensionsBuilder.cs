using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using NetRabbit.Extensions.Models;

namespace NetRabbit.Extensions.Builders
{
    internal static class ExtensionsBuilder
    {
        public static string Build(List<MessageHandlerInfo> classes)
        {
            var usings = @$"
            using NetRabbit.Models;
            {BuildNamespaces(classes)}
            namespace NetRabbit;";
            return usings + $@"
            
                public static class RabbitServicesExtensions
                {{
                    {BuildMethods(classes)}
                }}";
        }

        private static string BuildNamespaces(IReadOnlyCollection<MessageHandlerInfo> classes)
        {
            var sb = new StringBuilder();
            var genericArgNamespaces = classes
                                       .Where(x => x.GenericArg != default)
                                       .Select(x => x.GenericArg.Namespace);

            var handlerNamespaces = classes
                                    .Where(x => x.Handler != default)
                                    .Select(x => x.Handler.HandlerNamespace);

            foreach (var ns in genericArgNamespaces.Concat(handlerNamespaces).Distinct())
            {
                sb.AppendFormat(CultureInfo.InvariantCulture, "using {0};", ns);
            }
            return sb.ToString();
        }

        private static string BuildMethods(List<MessageHandlerInfo> classes)
        {
            var sb = new StringBuilder();

            sb.AppendFormat
            (
                CultureInfo.InvariantCulture,
                @"public static RabbitServices AddAllHandlers(this RabbitServices collection) 
                {{
                    {0}
                    return collection;
                }}",
                BuildAddAllHandlers(classes)
            ).AppendLine();

            static string BuildAddAllHandlers(IEnumerable<MessageHandlerInfo> classes)
            {
                return string.Join("\n", classes.Select(c => $"collection.{BuildMessageHandlerRegistration(c)}();"));
            }

            foreach (var @class in classes)
            {
                sb.AppendFormat
                (
                    CultureInfo.InvariantCulture,
                    @"public static RabbitServices Add{0}(this RabbitServices collection)
                      {{
                          return collection.{1}();
                      }}",
                    @class.Handler.HandlerName,
                    BuildMessageHandlerRegistration(@class)
                ).AppendLine();
            }

            return sb.ToString();
        }

        private static string BuildMessageHandlerRegistration(in MessageHandlerInfo @class)
        {
            if (@class.IsSyncHandler)
            {
                return string.IsNullOrEmpty(@class.GenericArg.GenericArgName)
                    ? $"AddSynchronizedMessageHandlerInternal<{@class.Handler.HandlerName}>"
                    : $"AddSynchronizedMessageHandlerInternal<{@class.Handler.HandlerName}, {@class.GenericArg.GenericArgName}>";
            }

            return string.IsNullOrEmpty(@class.GenericArg.GenericArgName)
                ? $"AddMessageHandlerInternal<{@class.Handler.HandlerName}>"
                : $"AddMessageHandlerInternal<{@class.Handler.HandlerName}, {@class.GenericArg.GenericArgName}>";
        }
    }
}
