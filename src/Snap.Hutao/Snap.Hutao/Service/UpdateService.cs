// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core;
using Snap.Hutao.Core.IO.Hashing;
using Snap.Hutao.Core.IO.Http.Sharding;
using Snap.Hutao.Service.Abstraction;
using Snap.Hutao.Web.Hutao;
using Snap.Hutao.Web.Hutao.Response;
using System.IO;
using System.Net.Http;

namespace Snap.Hutao.Service;

[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(IUpdateService))]
internal sealed partial class UpdateService : IUpdateService
{
    private readonly IServiceProvider serviceProvider;

    public async ValueTask<bool> InitializeAsync(IProgress<UpdateStatus> progress, CancellationToken token = default)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            ITaskContext taskContext = scope.ServiceProvider.GetRequiredService<ITaskContext>();
            await taskContext.SwitchToBackgroundAsync();

            HutaoInfrastructureClient infrastructureClient = serviceProvider.GetRequiredService<HutaoInfrastructureClient>();
            HutaoResponse<HutaoVersionInformation> response = await infrastructureClient.GetHutaoVersionInfomationAsync(token).ConfigureAwait(false);

            if (!response.IsOk())
            {
                return false;
            }

            HutaoVersionInformation versionInformation = response.Data;

            if (versionInformation.Sha256 is not { } sha256)
            {
                return false;
            }

            string dataFolder = scope.ServiceProvider.GetRequiredService<RuntimeOptions>().DataFolder;
            string msixPath = Path.Combine(dataFolder, "UpdateCache/Snap.Hutao.msix");

            if (await CheckUpdateCacheSHA256Async(msixPath, sha256, token).ConfigureAwait(false))
            {
                return true;
            }

            return await DownloadUpdatePackageAsync(versionInformation, msixPath, progress, token).ConfigureAwait(false);
        }
    }

    private static async ValueTask<bool> CheckUpdateCacheSHA256Async(string filePath, string remoteHash, CancellationToken token = default)
    {
        string localHash = await SHA256.HashFileAsync(filePath, token).ConfigureAwait(false);
        return string.Equals(localHash, remoteHash, StringComparison.OrdinalIgnoreCase);
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