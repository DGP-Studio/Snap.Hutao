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
using Windows.System;

namespace Snap.Hutao.Service.Update;

[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(IUpdateService))]
internal sealed partial class UpdateService : IUpdateService
{
    private const string UpdaterFilename = "Snap.Hutao.Deployment.exe";

    // Avoid injecting services directly
    private readonly IServiceProvider serviceProvider;

    public async ValueTask<CheckUpdateResult> CheckUpdateAsync(IProgress<UpdateStatus> progress, CancellationToken token = default)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            ITaskContext taskContext = scope.ServiceProvider.GetRequiredService<ITaskContext>();
            await taskContext.SwitchToBackgroundAsync();

            HutaoInfrastructureClient infrastructureClient = scope.ServiceProvider.GetRequiredService<HutaoInfrastructureClient>();
            HutaoResponse<HutaoPackageInformation> response = await infrastructureClient.GetHutaoVersionInformationAsync(token).ConfigureAwait(false);

            CheckUpdateResult checkUpdateResult = new();

            if (!ResponseValidator.TryValidate(response, scope.ServiceProvider, out HutaoPackageInformation? packageInformation))
            {
                checkUpdateResult.Kind = CheckUpdateResultKind.VersionApiInvalidResponse;
                return checkUpdateResult;
            }

            checkUpdateResult.Kind = CheckUpdateResultKind.NeedDownload;
            checkUpdateResult.PackageInformation = packageInformation;

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

                    checkUpdateResult.Kind = CheckUpdateResultKind.AlreadyUpdated;
                    return checkUpdateResult;
                }
            }

            // Insert CDN mirror if possible
            if (await scope.ServiceProvider.GetRequiredService<HutaoUserOptions>().GetIsHutaoCdnAllowedAsync().ConfigureAwait(false))
            {
                HutaoResponse<HutaoPackageMirror> mirrorResponse = await scope.ServiceProvider
                    .GetRequiredService<HutaoDistributionClient>()
                    .GetAcceleratedMirrorAsync($"Snap.Hutao.{packageInformation.Version.ToString(3)}.msix", token)
                    .ConfigureAwait(false);
                if (ResponseValidator.TryValidate(mirrorResponse, scope.ServiceProvider, out HutaoPackageMirror? mirror))
                {
                    checkUpdateResult.PackageInformation.Mirrors.Insert(0, mirror);
                }
            }

            progress.Report(new(checkUpdateResult.PackageInformation.Version.ToString(), 0, 0));

            if (checkUpdateResult.PackageInformation.Validation is not { Length: > 0 } sha256)
            {
                checkUpdateResult.Kind = CheckUpdateResultKind.VersionApiInvalidSha256;
                return checkUpdateResult;
            }

            // Whether the package does not exist or the hash is inconsistent, VersionApiInvalidSha256 will be returned
            if (File.Exists(msixPath) && await CheckUpdateCacheSha256Async(msixPath, sha256, token).ConfigureAwait(false))
            {
                checkUpdateResult.Kind = CheckUpdateResultKind.NeedInstall;
                return checkUpdateResult;
            }

            return checkUpdateResult;
        }
    }

    public async ValueTask<bool> DownloadUpdateAsync(HutaoSelectedMirrorInformation mirrorInformation, IProgress<UpdateStatus> progress, CancellationToken token = default)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            using (HttpClient httpClient = scope.ServiceProvider.GetRequiredService<HttpClient>())
            {
                HutaoPackageMirror mirror = mirrorInformation.Mirror;
                string version = mirrorInformation.Version.ToString();

                try
                {
                    using (HttpResponseMessage responseMessage = await httpClient.GetAsync(mirror.Url, HttpCompletionOption.ResponseHeadersRead, token).ConfigureAwait(false))
                    {
                        long totalBytes = responseMessage.Content.Headers.ContentLength ?? 0;
                        using (Stream contentStream = await responseMessage.Content.ReadAsStreamAsync(token).ConfigureAwait(false))
                        {
                            string filePath = HutaoRuntime.GetDataFolderUpdateCacheFolderFile("Snap.Hutao.msix");

                            switch (mirror.MirrorType)
                            {
                                case HutaoPackageMirrorType.Direct:
                                    using (FileStream fileStream = File.Create(filePath))
                                    {
                                        using (StreamCopyWorker<UpdateStatus> worker = new(contentStream, fileStream, (_, bytesRead) => new(version, bytesRead, totalBytes)))
                                        {
                                            await worker.CopyAsync(progress, token).ConfigureAwait(false);
                                        }
                                    }

                                    break;
                                case HutaoPackageMirrorType.Archive:
                                    using (TempFileStream tempFileStream = new(FileMode.Create, FileAccess.ReadWrite))
                                    {
                                        using (StreamCopyWorker<UpdateStatus> worker = new(contentStream, tempFileStream, (_, bytesRead) => new(version, bytesRead, totalBytes)))
                                        {
                                            await worker.CopyAsync(progress, token).ConfigureAwait(false);
                                        }

                                        using (ZipArchive archive = new(tempFileStream))
                                        {
                                            foreach (ZipArchiveEntry entry in archive.Entries)
                                            {
                                                if (!entry.FullName.EndsWith(".msix", StringComparison.OrdinalIgnoreCase))
                                                {
                                                    continue;
                                                }

                                                entry.ExtractToFile(filePath, true);
                                                break;
                                            }
                                        }
                                    }

                                    break;
                                case HutaoPackageMirrorType.Browser:
                                    await Launcher.LaunchUriAsync(mirror.Url.ToUri());

                                    // The download result should always fail, we deliberately do not handle it
                                    return false;
                            }

                            if (!File.Exists(filePath))
                            {
                                return false;
                            }

                            if (await CheckUpdateCacheSha256Async(filePath, mirrorInformation.Validation, token).ConfigureAwait(false))
                            {
                                return true;
                            }
                        }
                    }
                }
                catch
                {
                    // Ignore
                }

                return false;
            }
        }
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
            // The updater will request UAC permissions itself
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

    private static async ValueTask<bool> CheckUpdateCacheSha256Async(string filePath, string remoteHash, CancellationToken token = default)
    {
        string localHash = await Hash.FileToHexStringAsync(HashAlgorithmName.SHA256, filePath, token).ConfigureAwait(false);
        return string.Equals(localHash, remoteHash, StringComparison.OrdinalIgnoreCase);
    }
}