#tool "nuget:?package=nuget.commandline&version=6.9.1"
#addin nuget:?package=Cake.Http&version=4.0.0

var target = Argument("target", "Build");
var configuration = Argument("configuration", "Release");

// Pre-define

var version = "version";

var repoDir = "repoDir";
var outputPath = "outputPath";

var pw = "pw";

// Extension

static ProcessArgumentBuilder AppendIf(this ProcessArgumentBuilder builder, string text, bool condition)
{
    return condition ? builder.Append(text) : builder;
}

// Properties

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

if (GitHubActions.IsRunningOnGitHubActions)
{
    repoDir = GitHubActions.Environment.Workflow.Workspace.FullPath;
    outputPath = System.IO.Path.Combine(repoDir, "src", "output");

    if (GitHubActions.Environment.PullRequest.IsPullRequest)
    {
        version = System.DateTime.Now.ToString("yyyy.M.d.0");

        Information("Is Pull Request. Skip version.");
    }
    else
    {
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

        var certificateBase64 = HasEnvironmentVariable("CERTIFICATE") ? EnvironmentVariable("CERTIFICATE") : throw new Exception("Cannot find CERTIFICATE");
        pw = HasEnvironmentVariable("PW") ? EnvironmentVariable("PW") : throw new Exception("Cannot find PW");
        System.IO.File.WriteAllBytes(System.IO.Path.Combine(outputPath, "temp.pfx"), System.Convert.FromBase64String(certificateBase64));

        Information($"Version: {version}");
    }

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
else // Local
{
    repoDir = System.Environment.CurrentDirectory;
    outputPath = System.IO.Path.Combine(repoDir, "src", "output");

    version = System.DateTime.Now.ToString("yyyy.M.d.") + ((int)((System.DateTime.Now - System.DateTime.Today).TotalSeconds / 86400 * 65535)).ToString();

    Information($"Version: {version}");
}

// Windows SDK
var registry = new WindowsRegistry();
var winsdkRegistry = registry.LocalMachine.OpenKey(@"SOFTWARE\Microsoft\Windows Kits\Installed Roots");
var winsdkVersion = winsdkRegistry.GetSubKeyNames().MaxBy(key => int.Parse(key.Split(".")[2]));
var winsdkPath = (string)winsdkRegistry.GetValue("KitsRoot10");
var winsdkBinPath = System.IO.Path.Combine(winsdkPath, "bin", winsdkVersion, "x64");
Information($"Windows SDK: {winsdkPath}");

Task("Build")
    .IsDependentOn("Build binary package")
    .IsDependentOn("Copy files")
    .IsDependentOn("Build MSIX")
    .IsDependentOn("Sign");

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

    if (GitHubActions.IsRunningOnGitHubActions)
    {
        Information("Using CI configuraion");
        content = content
            .Replace("Snap Hutao", "Snap Hutao Alpha")
            .Replace("胡桃", "胡桃 Alpha")
            .Replace("DGP Studio", "DGP Studio CI");
        content = System.Text.RegularExpressions.Regex.Replace(content, "  Name=\"([^\"]*)\"", "  Name=\"7f0db578-026f-4e0b-a75b-d5d06bb0a74c\"");
        content = System.Text.RegularExpressions.Regex.Replace(content, "  Publisher=\"([^\"]*)\"", "  Publisher=\"E=admin@dgp-studio.cn, CN=DGP Studio CI, OU=CI, O=DGP-Studio, L=San Jose, S=CA, C=US\"");
        content = System.Text.RegularExpressions.Regex.Replace(content, "  Version=\"([0-9\\.]+)\"", $"  Version=\"{version}\"");
    }
    else if (AppVeyor.IsRunningOnAppVeyor)
    {
        Information("Using Release configuration");
        content = System.Text.RegularExpressions.Regex.Replace(content, "  Publisher=\"([^\"]*)\"", "  Publisher=\"CN=SignPath Foundation, O=SignPath Foundation, L=Lewes, S=Delaware, C=US\"");
    }
    else
    {
        Information("Using Local configuration.");
        content = content
            .Replace("Snap Hutao", "Snap Hutao Local")
            .Replace("胡桃", "胡桃 Local")
            .Replace("DGP Studio", "DGP Studio CI");
        content = System.Text.RegularExpressions.Regex.Replace(content, "  Name=\"([^\"]*)\"", "  Name=\"E8B6E2B3-D2A0-4435-A81D-2A16AAF405C7\"");
        content = System.Text.RegularExpressions.Regex.Replace(content, "  Publisher=\"([^\"]*)\"", "  Publisher=\"E=admin@dgp-studio.cn, CN=DGP Studio CI, OU=CI, O=DGP-Studio, L=San Jose, S=CA, C=US\"");
        content = System.Text.RegularExpressions.Regex.Replace(content, "  Version=\"([0-9\\.]+)\"", $"  Version=\"{version}\"");
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
                                            .AppendIf("/p:AlphaConstants=IS_ALPHA_BUILD", !AppVeyor.IsRunningOnAppVeyor)
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
    if (GitHubActions.IsRunningOnGitHubActions)
    {
        arguments = "pack /d " + binPath + " /p " + System.IO.Path.Combine(outputPath, $"Snap.Hutao.Alpha-{version}.msix");
    }
    else if (AppVeyor.IsRunningOnAppVeyor)
    {
        arguments = "pack /d " + binPath + " /p " + System.IO.Path.Combine(outputPath, $"Snap.Hutao-{version}.msix");
    }
    else
    {
        arguments = "pack /d " + binPath + " /p " + System.IO.Path.Combine(outputPath, $"Snap.Hutao.Local-{version}.msix");
    }

    var makeappxPath = System.IO.Path.Combine(winsdkBinPath, "makeappx.exe");

    var p = StartProcess(
        makeappxPath,
        new ProcessSettings
        {
            Arguments = arguments
        }
    );
    if (p != 0)
    {
        throw new InvalidOperationException("Build MSIX failed with exit code " + p);
    }
});

Task("Sign")
    .IsDependentOn("Build MSIX")
    .Does(() =>
{
    if (AppVeyor.IsRunningOnAppVeyor)
    {
        Information("Move to SignPath. Skip signing.");
        return;
    }
    else if (GitHubActions.IsRunningOnGitHubActions)
    {
        if (GitHubActions.Environment.PullRequest.IsPullRequest)
        {
            Information("Is Pull Request. Skip signing.");
            return;
        }

        var signPath = System.IO.Path.Combine(winsdkBinPath, "signtool.exe");
        var pfxPath = System.IO.Path.Combine(outputPath, "temp.pfx");
        var arguments = $"sign /debug /v /a /fd SHA256 /f {pfxPath} /p {pw} {System.IO.Path.Combine(outputPath, $"Snap.Hutao.Alpha-{version}.msix")}";

        var p = StartProcess(
            signPath,
            new ProcessSettings
            {
                Arguments = arguments
            }
        );
        if (p != 0)
        {
            throw new InvalidOperationException("Sign failed with exit code " + p);
        }
    }
    else
    {
        Information("Local configuration. Skip signing.");
        return;
    }
});

RunTarget(target);
