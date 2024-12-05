
#nullable enable
namespace NetRabbit.Extensions.Models
{
    internal readonly record struct MessageHandlerInfo
    (
        (string HandlerNamespace, string HandlerName) Handler,
        (string? GenericArgName, string? Namespace) GenericArg,
        bool IsSyncHandler
    );
}

namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit { }
}