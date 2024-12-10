using Nuke.Common;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;

[GitHubActions("nugetpush", GitHubActionsImage.UbuntuLatest, On = [GitHubActionsTrigger.WorkflowDispatch],
    InvokedTargets = [nameof(Push)],
    ImportSecrets = ["NugetApiKey"],
    AutoGenerate = true
)]
class Build : NukeBuild
{
    public static int Main() => Execute<Build>(x => x.Push);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Secret] readonly string NugetApiKey;
    [Parameter] string NugetApiUrl = "https://api.nuget.org/v3/index.json";

    readonly AbsolutePath ArtifactsDirectory = RootDirectory / ".nuke" / "artifacts";

    [Solution(GenerateProjects = true)] Solution Solution;

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            DotNetTasks.DotNetClean(s => s.SetConfiguration(Configuration.Release));
        });

    Target Restore => _ => _
        .DependsOn(Clean)
        .Executes(() =>
        {
            DotNetTasks.DotNetRestore();
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetTasks.DotNetBuild(s => s.SetConfiguration(Configuration.Release).EnableNoRestore());
        });

    Target Pack => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            ArtifactsDirectory.CreateOrCleanDirectory();
            DotNetTasks.DotNetPack
            (
                s => s
                .SetOutputDirectory(ArtifactsDirectory)
                .SetConfiguration(Configuration.Release)
                .EnableNoBuild()
                .EnableNoRestore()
                .DisableNoCache()
                .CombineWith
                (
                    [Solution.NetRabbit, Solution.NetRabbit_Extensions],
                    (settings, project) => settings.SetProject(project).SetConfiguration(Configuration.Release)
                )
             );
        });

    Target Push => _ => _
        .DependsOn(Pack)
        .Requires(() => NugetApiKey)
        .Requires(() => NugetApiUrl)
        .Executes(() =>
        {
            var packages = ArtifactsDirectory.GlobFiles("*.nupkg");
            DotNetTasks.DotNetNuGetPush
            (
                s => s
                    .SetApiKey(NugetApiKey)
                    .SetSource(NugetApiUrl)
                    .SetSkipDuplicate(true)
                    .CombineWith(packages, (settings, package) => settings.SetTargetPath(package))

            );
        });

}
