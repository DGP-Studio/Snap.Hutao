// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core;
using Snap.Hutao.Core.IO.Hashing;
using Snap.Hutao.Core.IO.Http.Sharding;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.Service.Abstraction;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.Web.Hutao;
using Snap.Hutao.Web.Hutao.Response;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using Windows.Storage;

namespace Snap.Hutao.Service.Update;

[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(IUpdateService))]
internal sealed partial class UpdateService : IUpdateService
{
    private const string UpdaterFilename = "Snap.Hutao.Deployment.exe";

    private readonly IServiceProvider serviceProvider;

    public async ValueTask<CheckUpdateResult> CheckUpdateAsync(IProgress<UpdateStatus> progress, CancellationToken token = default)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            ITaskContext taskContext = scope.ServiceProvider.GetRequiredService<ITaskContext>();
            await taskContext.SwitchToBackgroundAsync();

            HutaoInfrastructureClient infrastructureClient = scope.ServiceProvider.GetRequiredService<HutaoInfrastructureClient>();
            HutaoResponse<HutaoVersionInformation> response = await infrastructureClient.GetHutaoVersionInfomationAsync(token).ConfigureAwait(false);

            CheckUpdateResult checkUpdateResult = new();

            if (!response.IsOk())
            {
                checkUpdateResult.Kind = CheckUpdateResultKind.VersionApiInvalidResponse;
                return checkUpdateResult;
            }
            else
            {
                checkUpdateResult.Kind = CheckUpdateResultKind.NeedDownload;
                checkUpdateResult.HutaoVersionInformation = response.Data;
            }

            string msixPath = GetUpdatePackagePath();

            if (!LocalSetting.Get(SettingKeys.OverrideUpdateVersionComparison, false))
            {
                // Launched in an updated version
                if (scope.ServiceProvider.GetRequiredService<RuntimeOptions>().Version >= checkUpdateResult.HutaoVersionInformation.Version)
                {
                    if (File.Exists(msixPath))
                    {
                        File.Delete(msixPath);
                    }

                    checkUpdateResult.Kind = CheckUpdateResultKind.AlreayUpdated;
                    return checkUpdateResult;
                }
            }

            progress.Report(new(checkUpdateResult.HutaoVersionInformation.Version.ToString(), 0, 0));

            if (checkUpdateResult.HutaoVersionInformation.Sha256 is not { Length: > 0 } sha256)
            {
                checkUpdateResult.Kind = CheckUpdateResultKind.VersionApiInvalidSha256;
                return checkUpdateResult;
            }

            if (File.Exists(msixPath) && await CheckUpdateCacheSHA256Async(msixPath, sha256, token).ConfigureAwait(false))
            {
                checkUpdateResult.Kind = CheckUpdateResultKind.NeedInstall;
                return checkUpdateResult;
            }

            return checkUpdateResult;
        }
    }

    public ValueTask<bool> DownloadUpdateAsync(CheckUpdateResult checkUpdateResult, IProgress<UpdateStatus> progress, CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(checkUpdateResult.HutaoVersionInformation);
        return DownloadUpdatePackageAsync(checkUpdateResult.HutaoVersionInformation, GetUpdatePackagePath(), progress, token);
    }

    public LaunchUpdaterResult LaunchUpdater()
    {
        RuntimeOptions runtimeOptions = serviceProvider.GetRequiredService<RuntimeOptions>();
        string updaterTargetPath = runtimeOptions.GetDataFolderUpdateCacheFolderFile(UpdaterFilename);

        File.Copy(InstalledLocation.GetAbsolutePath(UpdaterFilename), updaterTargetPath, true);

        string commandLine = new CommandLineBuilder()
            .Append("--package-path", GetUpdatePackagePath(runtimeOptions))
            .Append("--family-name", runtimeOptions.FamilyName)
            .Append("--update-behavior", true)
            .ToString();

        try
        {
            Process? process = Process.Start(new ProcessStartInfo()
            {
                Arguments = commandLine,
                FileName = updaterTargetPath,
                UseShellExecute = true,
            });

            return new() { IsSuccess = true, Process = process };
        }
        catch (Exception ex)
        {
            serviceProvider.GetRequiredService<IInfoBarService>().Error(ex);
            return new() { IsSuccess = false };
        }
    }

    private static async ValueTask<bool> CheckUpdateCacheSHA256Async(string filePath, string remoteHash, CancellationToken token = default)
    {
        string localHash = await SHA256.HashFileAsync(filePath, token).ConfigureAwait(false);
        return string.Equals(localHash, remoteHash, StringComparison.OrdinalIgnoreCase);
    }

    private string GetUpdatePackagePath(RuntimeOptions? runtimeOptions = default)
    {
        runtimeOptions ??= serviceProvider.GetRequiredService<RuntimeOptions>();
        return runtimeOptions.GetDataFolderUpdateCacheFolderFile("Snap.Hutao.msix");
    }

    private async ValueTask<bool> DownloadUpdatePackageAsync(HutaoVersionInformation versionInformation, string filePath, IProgress<UpdateStatus> progress, CancellationToken token = default)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            using (HttpClient httpClient = scope.ServiceProvider.GetRequiredService<HttpClient>())
            {
                string version = versionInformation.Version.ToString();
                foreach (string url in versionInformation.Urls)
                {
                    HttpShardCopyWorkerOptions<UpdateStatus> options = new()
                    {
                        HttpClient = httpClient,
                        SourceUrl = url,
                        DestinationFilePath = filePath,
                        MaxDegreeOfParallelism = Math.Clamp(Environment.ProcessorCount, 2, 6),
                        StatusFactory = (bytesRead, totalBytes) => new UpdateStatus(version, bytesRead, totalBytes),
                    };

                    using (HttpShardCopyWorker<UpdateStatus> worker = await HttpShardCopyWorker<UpdateStatus>.CreateAsync(options).ConfigureAwait(false))
                    {
                        await worker.CopyAsync(progress, token).ConfigureAwait(false);
                    }

                    string? remoteHash = versionInformation.Sha256;
                    ArgumentNullException.ThrowIfNull(remoteHash);
                    if (await CheckUpdateCacheSHA256Async(filePath, remoteHash, token).ConfigureAwait(false))
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }
}