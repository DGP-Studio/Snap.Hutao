// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core;
using Snap.Hutao.Core.IO;
using Snap.Hutao.Core.IO.Hashing;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.Service.Hutao;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.Web.Hutao;
using Snap.Hutao.Web.Hutao.Response;
using Snap.Hutao.Web.Response;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Security.Cryptography;

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
            HutaoResponse<HutaoPackageInformation> response = await infrastructureClient.GetHutaoVersionInfomationAsync(token).ConfigureAwait(false);

            CheckUpdateResult checkUpdateResult = new();

            if (!ResponseValidator.TryValidate(response, scope.ServiceProvider, out HutaoPackageInformation? packageInformation))
            {
                checkUpdateResult.Kind = CheckUpdateResultKind.VersionApiInvalidResponse;
                return checkUpdateResult;
            }

            checkUpdateResult.Kind = CheckUpdateResultKind.NeedDownload;
            checkUpdateResult.PackageInformation = packageInformation;

            HutaoUserOptions hutaoUserOptions = scope.ServiceProvider.GetRequiredService<HutaoUserOptions>();
            if (await hutaoUserOptions.GetIsCloudServiceAllowedAsync().ConfigureAwait(false))
            {
                HutaoDistributionClient distributionClient = scope.ServiceProvider.GetRequiredService<HutaoDistributionClient>();
                HutaoResponse<HutaoPackageMirror> mirrorResponse = await distributionClient.GetAccMirrorAsync($"Snap.Hutao.{packageInformation.Version.ToString(3)}.msix", token).ConfigureAwait(false);
                if (mirrorResponse.Data is { } mirror)
                {
                    checkUpdateResult.PackageInformation.Mirrors.Add(mirror);
                }
            }

            string msixPath = HutaoRuntime.GetDataFolderUpdateCacheFolderFile("Snap.Hutao.msix");

            if (!LocalSetting.Get(SettingKeys.OverrideUpdateVersionComparison, false))
            {
                // Launched in an updated version
                if (HutaoRuntime.Version >= checkUpdateResult.PackageInformation.Version)
                {
                    if (File.Exists(msixPath))
                    {
                        File.Delete(msixPath);
                    }

                    checkUpdateResult.Kind = CheckUpdateResultKind.AlreayUpdated;
                    return checkUpdateResult;
                }
            }

            progress.Report(new(checkUpdateResult.PackageInformation.Version.ToString(), 0, 0));

            if (checkUpdateResult.PackageInformation.Validation is not { Length: > 0 } sha256)
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

    public ValueTask<bool> DownloadUpdateAsync(HutaoSelectedMirrorInformation mirrorInformation, IProgress<UpdateStatus> progress, CancellationToken token = default)
    {
        return DownloadUpdatePackageAsync(mirrorInformation, HutaoRuntime.GetDataFolderUpdateCacheFolderFile("Snap.Hutao.msix"), progress, token);
    }

    public LaunchUpdaterResult LaunchUpdater()
    {
        string updaterTargetPath = HutaoRuntime.GetDataFolderUpdateCacheFolderFile(UpdaterFilename);

        InstalledLocation.CopyFileFromApplicationUri($"ms-appx:///{UpdaterFilename}", updaterTargetPath);

        string commandLine = new CommandLineBuilder()
            .Append("--package-path", HutaoRuntime.GetDataFolderUpdateCacheFolderFile("Snap.Hutao.msix"))
            .Append("--family-name", HutaoRuntime.FamilyName)
            .Append("--update-behavior", true)
            .ToString();

        try
        {
            Process? process = Process.Start(new ProcessStartInfo
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
        string localHash = await Hash.FileToHexStringAsync(HashAlgorithmName.SHA256, filePath, token).ConfigureAwait(false);
        return string.Equals(localHash, remoteHash, StringComparison.OrdinalIgnoreCase);
    }

    private async ValueTask<bool> DownloadUpdatePackageAsync(HutaoSelectedMirrorInformation mirrorInformation, string filePath, IProgress<UpdateStatus> progress, CancellationToken token = default)
    {
        using IServiceScope scope = serviceProvider.CreateScope();
        using HttpClient httpClient = scope.ServiceProvider.GetRequiredService<HttpClient>();

        HutaoPackageMirror mirror = mirrorInformation.Mirror;
        string version = mirrorInformation.Version.ToString();

        try
        {
            using HttpResponseMessage responseMessage = await httpClient.GetAsync(mirror.Url, HttpCompletionOption.ResponseHeadersRead, token).ConfigureAwait(false);
            long totalBytes = responseMessage.Content.Headers.ContentLength ?? 0;
            using Stream webStream = await responseMessage.Content.ReadAsStreamAsync(token).ConfigureAwait(false);

            switch (mirror.MirrorType)
            {
                case HutaoPackageMirrorType.Direct:
                    using (FileStream fileStream = File.Create(filePath))
                    {
                        using (StreamCopyWorker<UpdateStatus> worker = new(webStream, fileStream, (_, bytesRead) => new UpdateStatus(version, bytesRead, totalBytes)))
                        {
                            await worker.CopyAsync(progress, token).ConfigureAwait(false);
                        }
                    }

                    break;
                case HutaoPackageMirrorType.Archive:
                    using (TempFileStream tempFileStream = new(FileMode.Create, FileAccess.ReadWrite))
                    {
                        using (StreamCopyWorker<UpdateStatus> worker = new(webStream, tempFileStream, (_, bytesRead) => new UpdateStatus(version, bytesRead, totalBytes)))
                        {
                            await worker.CopyAsync(progress, token).ConfigureAwait(false);
                        }

                        using ZipArchive archive = new(tempFileStream);
                        foreach (ZipArchiveEntry entry in archive.Entries)
                        {
                            if (!entry.FullName.EndsWith(".msix", StringComparison.OrdinalIgnoreCase))
                            {
                                continue;
                            }

                            using Stream entryStream = entry.Open();
                            using FileStream fileStream = File.Create(filePath);
                            await entryStream.CopyToAsync(fileStream, token).ConfigureAwait(false);
                            break;
                        }
                    }

                    break;
            }

            if (!File.Exists(filePath))
            {
                return false;
            }

            string remoteHash = mirrorInformation.Validation;
            if (await CheckUpdateCacheSHA256Async(filePath, remoteHash, token).ConfigureAwait(false))
            {
                return true;
            }
        }
        catch
        {
        }

        return false;
    }
}