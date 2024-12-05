using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Toolchains.InProcess.Emit;
namespace Benchmarks;
public class DontForceGcCollectionsConfig : ManualConfig
{
    public DontForceGcCollectionsConfig()
    {
        AddJob
        (
            Job.Default.WithGcMode(new GcMode { Force = false })
               .WithRuntime(BenchmarkDotNet.Environments.CoreRuntime.Core60)
               .WithId("InProcess")
               .WithToolchain(InProcessEmitToolchain.Instance)
        );
    }
}