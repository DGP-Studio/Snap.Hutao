#tool "nuget:?package=nuget.commandline&version=6.5.0"
#addin nuget:?package=Cake.Http&version=3.0.2

var target = Argument("target", "Build");
var configuration = Argument("configuration", "Release");

// Pre-define

var version = "version";

var repoDir = "repoDir";
var outputPath = "outputPath";

string solution
{
    get => System.IO.Path.Combine(repoDir, "src", "Snap.Hutao", "Snap.Hutao.sln");
}
string project
{
    get => System.IO.Path.Combine(repoDir, "src", "Snap.Hutao", "Snap.Hutao", "Snap.Hutao.csproj");
}
string binPath
{
    get => System.IO.Path.Combine(repoDir, "src", "Snap.Hutao", "Snap.Hutao", "bin", "x64", "Release", "net8.0-windows10.0.22621.0", "win-x64");
}
string manifest
{
    get => System.IO.Path.Combine(repoDir, "src", "Snap.Hutao", "Snap.Hutao", "Package.appxmanifest");
}

if (AzurePipelines.IsRunningOnAzurePipelines)
{
    repoDir = AzurePipelines.Environment.Build.SourcesDirectory.FullPath;
    outputPath = AzurePipelines.Environment.Build.ArtifactStagingDirectory.FullPath;

    var versionAuth = HasEnvironmentVariable("VERSION_API_TOKEN") ? EnvironmentVariable("VERSION_API_TOKEN") : throw new Exception("Cannot find VERSION_API_TOKEN");
    version = HttpGet(
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

    AzurePipelines.Commands.SetVariable("version", version);
}
else if (GitHubActions.IsRunningOnGitHubActions)
{
    repoDir = GitHubActions.Environment.Runner.Workspace.FullPath;
    outputPath = System.IO.Path.Combine(repoDir, "src", "output");

    var versionAuth = HasEnvironmentVariable("VERSION_API_TOKEN") ? EnvironmentVariable("VERSION_API_TOKEN") : throw new Exception("Cannot find VERSION_API_TOKEN");
    version = HttpGet(
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

    GitHubActions.Commands.SetOutputParameter("version", version);
}
else if (AppVeyor.IsRunningOnAppVeyor)
{
    repoDir = AppVeyor.Environment.Build.Folder;
    outputPath = System.IO.Path.Combine(repoDir, "src", "output");

    version = XmlPeek(manifest, "appx:Package/appx:Identity/@Version", new XmlPeekSettings
        {
            Namespaces = new Dictionary<string, string> { { "appx", "http://schemas.microsoft.com/appx/manifest/foundation/windows10" } }
        })[..^2];
    Information($"Version: {version}");
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

    var content = System.IO.File.ReadAllText(manifest);

    if (AzurePipelines.IsRunningOnAzurePipelines || GitHubActions.IsRunningOnGitHubActions)
    {
        Information("Using CI configuraion");
        content = content
            .Replace("Snap Hutao", "Snap Hutao Alpha")
            .Replace("胡桃", "胡桃 Alpha")
            .Replace("DGP Studio", "DGP Studio CI");
        content = System.Text.RegularExpressions.Regex.Replace(content, "  Name=\"([^\"]*)\"", "  Name=\"7f0db578-026f-4e0b-a75b-d5d06bb0a74c\"");
        content = System.Text.RegularExpressions.Regex.Replace(content, "  Publisher=\"([^\"]*)\"", "  Publisher=\"CN=DGP Studio CI\"");
        content = System.Text.RegularExpressions.Regex.Replace(content, "  Version=\"([0-9\\.]+)\"", $"  Version=\"{version}\"");
    }
    else if (AppVeyor.IsRunningOnAppVeyor)
    {
        Information("Using Release configuration");
        content = System.Text.RegularExpressions.Regex.Replace(content, "  Publisher=\"([^\"]*)\"", "  Publisher=\"CN=SignPath Foundation, O=SignPath Foundation, L=Lewes, S=Delaware, C=US\"");
    }

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
    var arguments = "arguments";
    if (AzurePipelines.IsRunningOnAzurePipelines)
    {
        arguments = "pack /d " + binPath + " /p " + System.IO.Path.Combine(outputPath, $"Snap.Hutao.Alpha-{version}.msix");
    }
    else if (AppVeyor.IsRunningOnAppVeyor)
    {
        arguments = "pack /d " + binPath + " /p " + System.IO.Path.Combine(outputPath, $"Snap.Hutao-{version}.msix");
    }
    var p = StartProcess(
        "makeappx.exe",
        new ProcessSettings
        {
            Arguments = arguments
        }
    );
    if (p != 0)
    {
        throw new InvalidOperationException("Build failed with exit code " + p);
    }
});

RunTarget(target);
