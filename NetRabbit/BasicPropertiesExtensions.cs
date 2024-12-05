using System.Collections.Generic;
using NetRabbit.Models;
using RabbitMQ.Client;

namespace NetRabbit
{
    internal static class BasicPropertiesExtensions
    {
        public static BasicMessageProperties ToBasicMessageProperties(this IReadOnlyBasicProperties basicProperties)
        {
            return new BasicMessageProperties
            {
                ContentEncoding = basicProperties.ContentEncoding,
                ContentType = basicProperties.ContentType,
                Headers = basicProperties.Headers as Dictionary<string, object>,
                AppId = basicProperties.AppId,
                DeliveryMode = (int)basicProperties.DeliveryMode,
                MessageId = basicProperties.MessageId
            };
        }
    }
}
