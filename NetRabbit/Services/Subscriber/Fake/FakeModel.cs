using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace NetRabbit.Services.Subscriber.Fake;

public sealed class FakeModel : IChannel
{
    //public void Dispose()
    //{
    //    throw new NotImplementedException();
    //}

    //public void Abort()
    //{

    //}

    //public void Abort(ushort replyCode, string replyText)
    //{

    //}

    //public void BasicAck(ulong deliveryTag, bool multiple)
    //{
    //    if (BasicAcks is null) return;

    //    foreach (EventHandler<BasicAckEventArgs> h in BasicAcks.GetInvocationList())
    //    {
    //        h(this, new BasicAckEventArgs(deliveryTag, multiple));
    //    }
    //}

    //public void BasicCancel(string consumerTag)
    //{

    //}

    //public void BasicCancelNoWait(string consumerTag)
    //{
    //    throw new NotImplementedException();
    //}

    //public string BasicConsume(string queue, bool autoAck, string consumerTag, bool noLocal, bool exclusive, IDictionary<string, object> arguments,
    //    IBasicConsumer consumer)
    //{
    //    return string.Empty;
    //}

    //public BasicGetResult BasicGet(string queue, bool autoAck)
    //{
    //    throw new NotImplementedException();
    //}

    //public void BasicNack(ulong deliveryTag, bool multiple, bool requeue)
    //{
    //    if (BasicNacks is null) return;

    //    foreach (EventHandler<BasicNackEventArgs> h in BasicNacks.GetInvocationList())
    //    {
    //        h(this, new BasicNackEventArgs(deliveryTag, multiple, requeue));
    //    }
    //}

    //public void BasicPublish(string exchange, string routingKey, bool mandatory, IBasicProperties basicProperties,
    //    ReadOnlyMemory<byte> body)
    //{

    //}

    //public void BasicQos(uint prefetchSize, ushort prefetchCount, bool global)
    //{

    //}

    //public void BasicRecover(bool requeue)
    //{

    //}

    //public void BasicRecoverAsync(bool requeue)
    //{

    //}

    //public void BasicReject(ulong deliveryTag, bool requeue)
    //{
    //    throw new NotImplementedException();
    //}

    //public void Close()
    //{
    //    throw new NotImplementedException();
    //}

    //public void Close(ushort replyCode, string replyText)
    //{
    //    throw new NotImplementedException();
    //}

    //public void ConfirmSelect()
    //{
    //    throw new NotImplementedException();
    //}

    //public IBasicPublishBatch CreateBasicPublishBatch()
    //{
    //    throw new NotImplementedException();
    //}

    //public IBasicProperties CreateBasicProperties()
    //{
    //    return new FakeBasicProperties();
    //}

    //public void ExchangeBind(string destination, string source, string routingKey, IDictionary<string, object> arguments)
    //{
    //    throw new NotImplementedException();
    //}

    //public void ExchangeBindNoWait(string destination, string source, string routingKey, IDictionary<string, object> arguments)
    //{
    //    throw new NotImplementedException();
    //}

    //public void ExchangeDeclare(string exchange, string type, bool durable, bool autoDelete, IDictionary<string, object> arguments)
    //{
    //    throw new NotImplementedException();
    //}

    //public void ExchangeDeclareNoWait(string exchange, string type, bool durable, bool autoDelete, IDictionary<string, object> arguments)
    //{
    //    throw new NotImplementedException();
    //}

    //public void ExchangeDeclarePassive(string exchange)
    //{
    //    throw new NotImplementedException();
    //}

    //public void ExchangeDelete(string exchange, bool ifUnused)
    //{
    //    throw new NotImplementedException();
    //}

    //public void ExchangeDeleteNoWait(string exchange, bool ifUnused)
    //{
    //    throw new NotImplementedException();
    //}

    //public void ExchangeUnbind(string destination, string source, string routingKey, IDictionary<string, object> arguments)
    //{
    //    throw new NotImplementedException();
    //}

    //public void ExchangeUnbindNoWait(string destination, string source, string routingKey, IDictionary<string, object> arguments)
    //{
    //    throw new NotImplementedException();
    //}

    //public void QueueBind(string queue, string exchange, string routingKey, IDictionary<string, object> arguments)
    //{
    //    throw new NotImplementedException();
    //}

    //public void QueueBindNoWait(string queue, string exchange, string routingKey, IDictionary<string, object> arguments)
    //{
    //    throw new NotImplementedException();
    //}

    //public QueueDeclareOk QueueDeclare(string queue, bool durable, bool exclusive, bool autoDelete, IDictionary<string, object> arguments)
    //{
    //    throw new NotImplementedException();
    //}

    //public void QueueDeclareNoWait(string queue, bool durable, bool exclusive, bool autoDelete, IDictionary<string, object> arguments)
    //{
    //    throw new NotImplementedException();
    //}

    //public QueueDeclareOk QueueDeclarePassive(string queue)
    //{
    //    throw new NotImplementedException();
    //}

    //public uint MessageCount(string queue)
    //{
    //    throw new NotImplementedException();
    //}

    //public uint ConsumerCount(string queue)
    //{
    //    throw new NotImplementedException();
    //}

    //public uint QueueDelete(string queue, bool ifUnused, bool ifEmpty)
    //{
    //    throw new NotImplementedException();
    //}

    //public void QueueDeleteNoWait(string queue, bool ifUnused, bool ifEmpty)
    //{
    //    throw new NotImplementedException();
    //}

    //public uint QueuePurge(string queue)
    //{
    //    throw new NotImplementedException();
    //}

    //public void QueueUnbind(string queue, string exchange, string routingKey, IDictionary<string, object> arguments)
    //{
    //    throw new NotImplementedException();
    //}

    //public void TxCommit()
    //{
    //    throw new NotImplementedException();
    //}

    //public void TxRollback()
    //{
    //    throw new NotImplementedException();
    //}

    //public void TxSelect()
    //{
    //    throw new NotImplementedException();
    //}

    //public bool WaitForConfirms()
    //{
    //    throw new NotImplementedException();
    //}

    //public bool WaitForConfirms(TimeSpan timeout)
    //{
    //    throw new NotImplementedException();
    //}

    //public bool WaitForConfirms(TimeSpan timeout, out bool timedOut)
    //{
    //    throw new NotImplementedException();
    //}

    //public void WaitForConfirmsOrDie()
    //{
    //    throw new NotImplementedException();
    //}

    //public void WaitForConfirmsOrDie(TimeSpan timeout)
    //{
    //    throw new NotImplementedException();
    //}

    //public int ChannelNumber { get; }
    //public ShutdownEventArgs? CloseReason { get; }
    //public IBasicConsumer? DefaultConsumer { get; set; }
    //public bool IsClosed { get; }
    //public bool IsOpen { get; }
    //public ulong NextPublishSeqNo { get; }
    //public string? CurrentQueue { get; }
    //public TimeSpan ContinuationTimeout { get; set; }
    //public event EventHandler<BasicAckEventArgs>? BasicAcks;
    //public event EventHandler<BasicNackEventArgs>? BasicNacks;
    //public event EventHandler<EventArgs>? BasicRecoverOk;
    //public event EventHandler<BasicReturnEventArgs>? BasicReturn;
    //public event EventHandler<CallbackExceptionEventArgs>? CallbackException;
    //public event EventHandler<FlowControlEventArgs>? FlowControl;
    //public event EventHandler<ShutdownEventArgs>? ModelShutdown;
    public ValueTask DisposeAsync()
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }

    public ValueTask<ulong> GetNextPublishSequenceNumberAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }

    public ValueTask BasicAckAsync(ulong deliveryTag, bool multiple, CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }

    public ValueTask BasicNackAsync
        (ulong deliveryTag, bool multiple, bool requeue, CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }

    public Task BasicCancelAsync
        (string consumerTag, bool noWait = false, CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }

    public Task<string> BasicConsumeAsync
    (
        string queue, bool autoAck, string consumerTag, bool noLocal, bool exclusive, IDictionary<string, object?>? arguments,
        IAsyncBasicConsumer consumer, CancellationToken cancellationToken = new CancellationToken()
    )
    {
        throw new NotImplementedException();
    }

    public Task<BasicGetResult?> BasicGetAsync(string queue, bool autoAck, CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }

    public ValueTask BasicPublishAsync<TProperties>
    (
        string exchange, string routingKey, bool mandatory, TProperties basicProperties, ReadOnlyMemory<byte> body,
        CancellationToken cancellationToken = new CancellationToken()
    ) where TProperties : IReadOnlyBasicProperties, IAmqpHeader
    {
        throw new NotImplementedException();
    }

    public ValueTask BasicPublishAsync<TProperties>
    (
        CachedString exchange, CachedString routingKey, bool mandatory, TProperties basicProperties, ReadOnlyMemory<byte> body,
        CancellationToken cancellationToken = new CancellationToken()
    ) where TProperties : IReadOnlyBasicProperties, IAmqpHeader
    {
        throw new NotImplementedException();
    }

    public Task BasicQosAsync
        (uint prefetchSize, ushort prefetchCount, bool global, CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }

    public ValueTask BasicRejectAsync(ulong deliveryTag, bool requeue, CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }

    public Task CloseAsync
        (ushort replyCode, string replyText, bool abort, CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }

    public Task CloseAsync(ShutdownEventArgs reason, bool abort, CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }

    public Task ExchangeDeclareAsync
    (
        string exchange, string type, bool durable, bool autoDelete, IDictionary<string, object?>? arguments = null, bool passive = false,
        bool noWait = false, CancellationToken cancellationToken = new CancellationToken()
    )
    {
        throw new NotImplementedException();
    }

    public Task ExchangeDeclarePassiveAsync(string exchange, CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }

    public Task ExchangeDeleteAsync
    (
        string exchange, bool ifUnused = false, bool noWait = false,
        CancellationToken cancellationToken = new CancellationToken()
    )
    {
        throw new NotImplementedException();
    }

    public Task ExchangeBindAsync
    (
        string destination, string source, string routingKey, IDictionary<string, object?>? arguments = null, bool noWait = false,
        CancellationToken cancellationToken = new CancellationToken()
    )
    {
        throw new NotImplementedException();
    }

    public Task ExchangeUnbindAsync
    (
        string destination, string source, string routingKey, IDictionary<string, object?>? arguments = null, bool noWait = false,
        CancellationToken cancellationToken = new CancellationToken()
    )
    {
        throw new NotImplementedException();
    }

    public Task<QueueDeclareOk> QueueDeclareAsync
    (
        string queue, bool durable, bool exclusive, bool autoDelete, IDictionary<string, object?>? arguments = null, bool passive = false,
        bool noWait = false, CancellationToken cancellationToken = new CancellationToken()
    )
    {
        throw new NotImplementedException();
    }

    public Task<QueueDeclareOk> QueueDeclarePassiveAsync(string queue, CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }

    public Task<uint> QueueDeleteAsync
    (
        string queue, bool ifUnused, bool ifEmpty, bool noWait = false,
        CancellationToken cancellationToken = new CancellationToken()
    )
    {
        throw new NotImplementedException();
    }

    public Task<uint> QueuePurgeAsync(string queue, CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }

    public Task QueueBindAsync
    (
        string queue, string exchange, string routingKey, IDictionary<string, object?>? arguments = null, bool noWait = false,
        CancellationToken cancellationToken = new CancellationToken()
    )
    {
        throw new NotImplementedException();
    }

    public Task QueueUnbindAsync
    (
        string queue, string exchange, string routingKey, IDictionary<string, object?>? arguments = null,
        CancellationToken cancellationToken = new CancellationToken()
    )
    {
        throw new NotImplementedException();
    }

    public Task<uint> MessageCountAsync(string queue, CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }

    public Task<uint> ConsumerCountAsync(string queue, CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }

    public Task TxCommitAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }

    public Task TxRollbackAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }

    public Task TxSelectAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }

    public int ChannelNumber { get; }
    public ShutdownEventArgs? CloseReason { get; }
    public IAsyncBasicConsumer? DefaultConsumer { get; set; }
    public bool IsClosed { get; }
    public bool IsOpen { get; }
    public string? CurrentQueue { get; }
    public TimeSpan ContinuationTimeout { get; set; }
    public event AsyncEventHandler<BasicAckEventArgs>? BasicAcksAsync;
    public event AsyncEventHandler<BasicNackEventArgs>? BasicNacksAsync;
    public event AsyncEventHandler<BasicReturnEventArgs>? BasicReturnAsync;
    public event AsyncEventHandler<CallbackExceptionEventArgs>? CallbackExceptionAsync;
    public event AsyncEventHandler<FlowControlEventArgs>? FlowControlAsync;
    public event AsyncEventHandler<ShutdownEventArgs>? ChannelShutdownAsync;
}