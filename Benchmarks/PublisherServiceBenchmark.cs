using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using NetRabbit.Models;
using NetRabbit.Services.Publisher;
using NetRabbit.Services.Subscriber.Fake;
using NetRabbit.Settings;

namespace Benchmarks
{
    [Config(typeof(DontForceGcCollectionsConfig))]
    [MemoryDiagnoser]
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    public class PublisherServiceBenchmark
    {
        private static readonly PublisherService Publisher = new(new BasicConnectionSettings(), new FakeChannelFactory());

        [Benchmark]
        public static Task<PublishResponseMessage> Publish()
        {
            return Publisher.PublishAsync(new PublisherBrokeredMessage<string>("Example"), "exchange");
        }

        [Benchmark]
        public static async Task<PublishResponseMessage> PublishConfirm()
        {
            var task = Publisher.PublishConfirmAsync(new PublisherBrokeredMessage<string>("Hello"), "exchange");
            await FakeChannelFactory.GetModel().BasicAckAsync(1, true);
            return await task;
        }
    }
}
