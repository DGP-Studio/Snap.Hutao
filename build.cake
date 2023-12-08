#tool "nuget:?package=nuget.commandline&version=6.5.0"
var target = Argument("target", "Build");
var configuration = Argument("configuration", "Release");

var version = HasArgument("Version") ? Argument<string>("Version") : throw new Exception("Empty Version");
var pw = HasArgument("pw") ? Argument<string>("pw") : throw new Exception("Empty pw");

// Default Azure Pipelines

var repoDir = AzurePipelines.Environment.Build.SourcesDirectory.FullPath;
var outputPath = AzurePipelines.Environment.Build.ArtifactStagingDirectory.FullPath;

var solution = System.IO.Path.Combine(repoDir, "src", "Snap.Hutao", "Snap.Hutao.sln");
var project = System.IO.Path.Combine(repoDir, "src", "Snap.Hutao", "Snap.Hutao", "Snap.Hutao.csproj");
var binPath = System.IO.Path.Combine(repoDir, "src", "Snap.Hutao", "Snap.Hutao", "bin", "x64", "Release", "net8.0-windows10.0.22621.0", "win-x64");

var pfxFile = System.IO.Path.Combine(AzurePipelines.Environment.Agent.HomeDirectory.FullPath, "_work", "_temp", "DGP_Studio_CI.pfx");

Task("Build")
    .IsDependentOn("Build binary package")
    .IsDependentOn("Copy files")
    .IsDependentOn("Build MSIX")
    .IsDependentOn("Sign MSIX");

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

Task("Sign MSIX")
    .IsDependentOn("Build MSIX")
    .Does(() =>
{
    var p = StartProcess(
        "signtool.exe",
        new ProcessSettings
        {
            Arguments = "sign /debug /v /a /fd SHA256 /f " + pfxFile + " /p " + pw + " " + System.IO.Path.Combine(outputPath, $"Snap.Hutao.Alpha-{version}.msix")
        }
    );
    if (p != 0)
    {
        throw new InvalidOperationException("Build failed with exit code " + p);
    }
});

RunTarget(target);
