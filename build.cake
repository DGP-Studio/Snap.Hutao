#tool "nuget:?package=nuget.commandline&version=6.5.0"
var target = Argument("target", "Build");
var configuration = Argument("configuration", "Release");

var version = HasArgument("Version") ? Argument<string>("Version") : throw new Exception("Empty Version");
var pw = HasArgument("pw") ? Argument<string>("pw") : throw new Exception("Empty pw");

var solution = System.IO.Path.Combine(AzurePipelines.Environment.Build.SourcesDirectory.FullPath, "src", "Snap.Hutao", "Snap.Hutao.sln");
var project = System.IO.Path.Combine(AzurePipelines.Environment.Build.SourcesDirectory.FullPath, "src", "Snap.Hutao", "Snap.Hutao", "Snap.Hutao.csproj");
var binPath = System.IO.Path.Combine(AzurePipelines.Environment.Build.SourcesDirectory.FullPath, "src", "Snap.Hutao", "Snap.Hutao", "bin", "x64", "Release", "net8.0-windows10.0.22621.0", "win-x64");
var outputPath = AzurePipelines.Environment.Build.ArtifactStagingDirectory.FullPath;

Task("Build")
    .IsDependentOn("Build binary package")
    .IsDependentOn("Copy files")
    .IsDependentOn("Build MSIX")
    .IsDependentOn("Sign MSIX");

Task("NuGet Restore")
    .Does(() =>
{
    Information("Restoring packages...");
    DotNetRestore(project, new DotNetRestoreSettings
    {
        Verbosity = DotNetVerbosity.Detailed,
        Interactive = false,
        ConfigFile = System.IO.Path.Combine(AzurePipelines.Environment.Build.SourcesDirectory.FullPath, "NuGet.Config")
    });
});

Task("Generate AppxManifest")
    .Does(() =>
{
    Information("Generating AppxManifest...");

    var manifestPath = System.IO.Path.Combine(AzurePipelines.Environment.Build.SourcesDirectory.FullPath, "src", "Snap.Hutao", "Snap.Hutao", "Package.appxmanifest");
    var manifest = System.IO.File.ReadAllText(manifestPath);

    manifest = manifest
        .Replace("Snap Hutao", "Snap Hutao Alpha")
        .Replace("胡桃", "胡桃 Alpha")
        .Replace("DGP Studio", "DGP Studio CI");
    manifest = System.Text.RegularExpressions.Regex.Replace(manifest, "  Name=\"([^\"]*)\"", "  Name=\"7f0db578-026f-4e0b-a75b-d5d06bb0a74c\"");
    manifest = System.Text.RegularExpressions.Regex.Replace(manifest, "  Publisher=\"([^\"]*)\"", "  Publisher=\"CN=DGP Studio CI\"");
    manifest = System.Text.RegularExpressions.Regex.Replace(manifest, "  Version=\"([0-9\\.]+)\"", $"  Version=\"{version}\"");

    System.IO.File.WriteAllText(manifestPath, manifest);

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
        System.IO.Path.Combine(AzurePipelines.Environment.Build.SourcesDirectory.FullPath, "src", "Snap.Hutao", "Snap.Hutao", "Assets"),
        System.IO.Path.Combine(binPath, "Assets")
    );

    Information("Copying resource...");
    CopyDirectory(
        System.IO.Path.Combine(AzurePipelines.Environment.Build.SourcesDirectory.FullPath, "src", "Snap.Hutao", "Snap.Hutao", "Resource"),
        System.IO.Path.Combine(binPath, "Resource")
    );
});

Task("Build MSIX")
    .IsDependentOn("Build binary package")
    .IsDependentOn("Copy files")
    .Does(() =>
{
    var p = StartProcess(
        "C:\\Program Files (x86)\\Windows Kits\\10\\bin\\10.0.22621.0\\x64\\makeappx.exe",
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
        "C:\\Program Files (x86)\\Windows Kits\\10\\bin\\10.0.22621.0\\x64\\signtool.exe",
        new ProcessSettings
        {
            Arguments = "sign /debug /v /a /fd SHA256 /f " + System.IO.Path.Combine(AzurePipelines.Environment.Agent.HomeDirectory.FullPath, "_work", "_temp", "DGP_Studio_CI.pfx") + " /p " + pw + " " + System.IO.Path.Combine(outputPath, $"Snap.Hutao.Alpha-{version}.msix")
        }
    );
    if (p != 0)
    {
        throw new InvalidOperationException("Build failed with exit code " + p);
    }
});

RunTarget(target);
