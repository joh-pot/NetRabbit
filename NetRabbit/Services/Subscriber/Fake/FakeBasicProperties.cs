using System;
using System.Collections.Generic;
using RabbitMQ.Client;

namespace NetRabbit.Services.Subscriber.Fake
{
    internal class FakeBasicProperties : IBasicProperties
    {
        private string? _appId;
        private string? _clusterId;
        private string? _contentEncoding;
        private string? _contentType;
        private string? _correlationId;
        private DeliveryModes _deliveryMode;
        private string? _expiration;
        private IDictionary<string, object?>? _headers;
        private string? _messageId;
        private bool _persistent;
        private byte _priority;
        private string? _replyTo;
        private PublicationAddress? _replyToAddress;
        private AmqpTimestamp _timestamp;
        private string? _type;
        private string? _userId;
        private string? _appId1;
        private string? _clusterId1;
        private string? _contentEncoding1;
        private string? _contentType1;
        private string? _correlationId1;
        private DeliveryModes _deliveryMode1;
        private string? _expiration1;
        private IDictionary<string, object?>? _headers1;
        private string? _messageId1;
        private bool _persistent1;
        private byte _priority1;
        private string? _replyTo1;
        private PublicationAddress? _replyToAddress1;
        private AmqpTimestamp _timestamp1;
        private string? _type1;
        private string? _userId1;

        public bool IsAppIdPresent()
        {
            throw new NotImplementedException();
        }

        public bool IsClusterIdPresent()
        {
            throw new NotImplementedException();
        }

        public bool IsContentEncodingPresent()
        {
            throw new NotImplementedException();
        }

        public bool IsContentTypePresent()
        {
            throw new NotImplementedException();
        }

        public bool IsCorrelationIdPresent()
        {
            throw new NotImplementedException();
        }

        public bool IsDeliveryModePresent()
        {
            throw new NotImplementedException();
        }

        public bool IsExpirationPresent()
        {
            throw new NotImplementedException();
        }

        public bool IsHeadersPresent()
        {
            throw new NotImplementedException();
        }

        public bool IsMessageIdPresent()
        {
            throw new NotImplementedException();
        }

        public bool IsPriorityPresent()
        {
            throw new NotImplementedException();
        }

        public bool IsReplyToPresent()
        {
            throw new NotImplementedException();
        }

        public bool IsTimestampPresent()
        {
            throw new NotImplementedException();
        }

        public bool IsTypePresent()
        {
            throw new NotImplementedException();
        }

        public bool IsUserIdPresent()
        {
            throw new NotImplementedException();
        }

        string? IBasicProperties.AppId
        {
            get => _appId1;
            set => _appId1 = value;
        }

        string? IBasicProperties.ClusterId
        {
            get => _clusterId1;
            set => _clusterId1 = value;
        }

        string? IBasicProperties.ContentEncoding
        {
            get => _contentEncoding1;
            set => _contentEncoding1 = value;
        }

        string? IBasicProperties.ContentType
        {
            get => _contentType1;
            set => _contentType1 = value;
        }

        string? IBasicProperties.CorrelationId
        {
            get => _correlationId1;
            set => _correlationId1 = value;
        }

        DeliveryModes IBasicProperties.DeliveryMode
        {
            get => _deliveryMode1;
            set => _deliveryMode1 = value;
        }

        string? IBasicProperties.Expiration
        {
            get => _expiration1;
            set => _expiration1 = value;
        }

        IDictionary<string, object?>? IBasicProperties.Headers
        {
            get => _headers1;
            set => _headers1 = value;
        }

        string? IBasicProperties.MessageId
        {
            get => _messageId1;
            set => _messageId1 = value;
        }

        bool IBasicProperties.Persistent
        {
            get => _persistent1;
            set => _persistent1 = value;
        }

        byte IBasicProperties.Priority
        {
            get => _priority1;
            set => _priority1 = value;
        }

        string? IBasicProperties.ReplyTo
        {
            get => _replyTo1;
            set => _replyTo1 = value;
        }

        PublicationAddress? IBasicProperties.ReplyToAddress
        {
            get => _replyToAddress1;
            set => _replyToAddress1 = value;
        }

        AmqpTimestamp IBasicProperties.Timestamp
        {
            get => _timestamp1;
            set => _timestamp1 = value;
        }

        string? IBasicProperties.Type
        {
            get => _type1;
            set => _type1 = value;
        }

        string? IBasicProperties.UserId
        {
            get => _userId1;
            set => _userId1 = value;
        }

        public void ClearAppId()
        {
            throw new NotImplementedException();
        }

        public void ClearClusterId()
        {
            throw new NotImplementedException();
        }

        public void ClearContentEncoding()
        {
            throw new NotImplementedException();
        }

        public void ClearContentType()
        {
            throw new NotImplementedException();
        }

        public void ClearCorrelationId()
        {
            throw new NotImplementedException();
        }

        public void ClearDeliveryMode()
        {
            throw new NotImplementedException();
        }

        public void ClearExpiration()
        {
            throw new NotImplementedException();
        }

        public void ClearHeaders()
        {
            throw new NotImplementedException();
        }

        public void ClearMessageId()
        {
            throw new NotImplementedException();
        }

        public void ClearPriority()
        {
            throw new NotImplementedException();
        }

        public void ClearReplyTo()
        {
            throw new NotImplementedException();
        }

        public void ClearTimestamp()
        {
            throw new NotImplementedException();
        }

        public void ClearType()
        {
            throw new NotImplementedException();
        }

        public void ClearUserId()
        {
            throw new NotImplementedException();
        }

        string? IReadOnlyBasicProperties.AppId => _appId;

        string? IReadOnlyBasicProperties.ClusterId => _clusterId;

        string? IReadOnlyBasicProperties.ContentEncoding => _contentEncoding;

        string? IReadOnlyBasicProperties.ContentType => _contentType;

        string? IReadOnlyBasicProperties.CorrelationId => _correlationId;

        DeliveryModes IReadOnlyBasicProperties.DeliveryMode => _deliveryMode;

        string? IReadOnlyBasicProperties.Expiration => _expiration;

        IDictionary<string, object?>? IReadOnlyBasicProperties.Headers => _headers;

        string? IReadOnlyBasicProperties.MessageId => _messageId;

        bool IReadOnlyBasicProperties.Persistent => _persistent;

        byte IReadOnlyBasicProperties.Priority => _priority;

        string? IReadOnlyBasicProperties.ReplyTo => _replyTo;

        PublicationAddress? IReadOnlyBasicProperties.ReplyToAddress => _replyToAddress;

        AmqpTimestamp IReadOnlyBasicProperties.Timestamp => _timestamp;

        string? IReadOnlyBasicProperties.Type => _type;

        string? IReadOnlyBasicProperties.UserId => _userId;
    }
}
