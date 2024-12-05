using BenchmarkDotNet.Running;
using Benchmarks;

//BenchmarkRunner.Run<PublisherServiceBenchmark>();

BenchmarkRunner.Run<SubscriberServiceBenchmark>();
