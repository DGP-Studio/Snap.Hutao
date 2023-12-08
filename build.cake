#tool "nuget:?package=nuget.commandline&version=6.5.0"
#addin nuget:?package=Cake.Http&version=3.0.2

var target = Argument("target", "Build");
var configuration = Argument("configuration", "Release");

var versionAuth = HasEnvironmentVariable("VERSION_API_TOKEN") ? EnvironmentVariable("VERSION_API_TOKEN") : throw new Exception("Cannot find VERSION_API_TOKEN");
var version = HttpGet(
    "https://internal.snapgenshin.cn/BuildIntergration/RequestNewVersion",
    new HttpSettings
    {
        Headers = new Dictionary<string, string>
            {
                { "Authorization", versionAuth }
            }
    }
    );
Information($"Version: {version}");

// Pre-define

var repoDir = "repoDir";
var outputPath = "outputPath";

var solution = "solution";
var project = "project";
var binPath = "binPath";

var pfxFile = "pfxFile";

if (AzurePipelines.IsRunningOnAzurePipelines)
{
    AzurePipelines.Commands.SetVariable("version", version);

    repoDir = AzurePipelines.Environment.Build.SourcesDirectory.FullPath;
    outputPath = AzurePipelines.Environment.Build.ArtifactStagingDirectory.FullPath;

    solution = System.IO.Path.Combine(repoDir, "src", "Snap.Hutao", "Snap.Hutao.sln");
    project = System.IO.Path.Combine(repoDir, "src", "Snap.Hutao", "Snap.Hutao", "Snap.Hutao.csproj");
    binPath = System.IO.Path.Combine(repoDir, "src", "Snap.Hutao", "Snap.Hutao", "bin", "x64", "Release", "net8.0-windows10.0.22621.0", "win-x64");

    pfxFile = System.IO.Path.Combine(AzurePipelines.Environment.Agent.HomeDirectory.FullPath, "_work", "_temp", "DGP_Studio_CI.pfx");
}
else if (AppVeyor.IsRunningOnAppVeyor)
{
    throw new NotImplementedException();
}

Task("Build")
    .IsDependentOn("Build binary package")
    .IsDependentOn("Copy files")
    .IsDependentOn("Build MSIX");

Task("NuGet Restore")
    .Does(() =>
{
    Information("Restoring packages...");

    var nugetConfig = System.IO.Path.Combine(repoDir, "NuGet.Config");
    DotNetRestore(project, new DotNetRestoreSettings
    {
        Verbosity = DotNetVerbosity.Detailed,
        Interactive = false,
        ConfigFile = nugetConfig
    });
});

Task("Generate AppxManifest")
    .Does(() =>
{
    Information("Generating AppxManifest...");

    var manifest = System.IO.Path.Combine(repoDir, "src", "Snap.Hutao", "Snap.Hutao", "Package.appxmanifest");
    var content = System.IO.File.ReadAllText(manifest);

    content = content
        .Replace("Snap Hutao", "Snap Hutao Alpha")
        .Replace("胡桃", "胡桃 Alpha")
        .Replace("DGP Studio", "DGP Studio CI");
    content = System.Text.RegularExpressions.Regex.Replace(content, "  Name=\"([^\"]*)\"", "  Name=\"7f0db578-026f-4e0b-a75b-d5d06bb0a74c\"");
    content = System.Text.RegularExpressions.Regex.Replace(content, "  Publisher=\"([^\"]*)\"", "  Publisher=\"CN=DGP Studio CI\"");
    content = System.Text.RegularExpressions.Regex.Replace(content, "  Version=\"([0-9\\.]+)\"", $"  Version=\"{version}\"");

    System.IO.File.WriteAllText(manifest, content);

    Information("Generated.");
});

Task("Build binary package")
    .IsDependentOn("NuGet Restore")
    .IsDependentOn("Generate AppxManifest")
    .Does(() =>
{
    Information("Building binary package...");

    var settings = new DotNetBuildSettings
    {
        Configuration = configuration
    };

    settings.MSBuildSettings = new DotNetMSBuildSettings
    {
        ArgumentCustomization = args => args.Append("/p:Platform=x64")
                                            .Append("/p:UapAppxPackageBuildMode=SideloadOnly")
                                            .Append("/p:AppxPackageSigningEnabled=false")
                                            .Append("/p:AppxBundle=Never")
                                            .Append("/p:AppxPackageOutput=" + outputPath)
    };

    DotNetBuild(project, settings);
});

Task("Copy files")
    .IsDependentOn("Build binary package")
    .Does(() =>
{
    Information("Copying assets...");
    CopyDirectory(
        System.IO.Path.Combine(repoDir, "src", "Snap.Hutao", "Snap.Hutao", "Assets"),
        System.IO.Path.Combine(binPath, "Assets")
    );

    Information("Copying resource...");
    CopyDirectory(
        System.IO.Path.Combine(repoDir, "src", "Snap.Hutao", "Snap.Hutao", "Resource"),
        System.IO.Path.Combine(binPath, "Resource")
    );
});

Task("Build MSIX")
    .IsDependentOn("Build binary package")
    .IsDependentOn("Copy files")
    .Does(() =>
{
    var p = StartProcess(
        "makeappx.exe",
        new ProcessSettings
        {
            Arguments = "pack /d " + binPath + " /p " + System.IO.Path.Combine(outputPath, $"Snap.Hutao.Alpha-{version}.msix")
        }
    );
    if (p != 0)
    {
        throw new InvalidOperationException("Build failed with exit code " + p);
    }
});

RunTarget(target);
