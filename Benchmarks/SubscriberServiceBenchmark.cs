using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using NetRabbit.Models;
using NetRabbit.Services.Subscriber;
using NetRabbit.Services.Subscriber.ConsumerServices;
using NetRabbit.Services.Subscriber.Fake;
using NetRabbit.Settings;

namespace Benchmarks;

[Config(typeof(DontForceGcCollectionsConfig))]
[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
public class SubscriberServiceBenchmark
{
    private static readonly FakeMessageHandler Handler = new();
    private static readonly SubscriberSettings SubscriberSettings = new("", "", 1);
    private MessageHandlerSubscriberService? _subscriberService;
    private IRabbitConsumer? _consumer;

    [GlobalSetup]
    public async Task Setup()
    {
        _subscriberService = new MessageHandlerSubscriberService
        (
            new FakeChannelFactory(),
            new MessageHandlerConsumerService()
        );

        _consumer = await _subscriberService.StartAsync
        (
            SubscriberSettings,
            new ConnectionSettings(new BasicConnectionSettings()),
            Handler
        );
    }

    [Benchmark]
    public void Run()
    {
        _consumer!.MessageDelivered();
    }
}