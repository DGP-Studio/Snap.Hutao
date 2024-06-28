// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Extensions.Caching.Memory;
using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Core.Diagnostics;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Core.IO;
using Snap.Hutao.Core.IO.Hashing;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.Service.Notification;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;

namespace Snap.Hutao.Service.Metadata;

/// <summary>
/// 元数据服务
/// </summary>
[HighQuality]
[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(IMetadataService))]
[HttpClient(HttpClientConfiguration.Default)]
internal sealed partial class MetadataService : IMetadataService, IMetadataServiceInitialization
{
    private const string MetaFileName = "Meta.json";

    private readonly TaskCompletionSource initializeCompletionSource = new();

    private readonly IServiceScopeFactory serviceScopeFactory;
    private readonly ILogger<MetadataService> logger;
    private readonly MetadataOptions metadataOptions;
    private readonly IInfoBarService infoBarService;
    private readonly JsonSerializerOptions options;
    private readonly IMemoryCache memoryCache;

    private bool isInitialized;

    public IMemoryCache MemoryCache { get => memoryCache; }

    public async ValueTask<bool> InitializeAsync()
    {
        await initializeCompletionSource.Task.ConfigureAwait(false);
        return isInitialized;
    }

    public async ValueTask InitializeInternalAsync(CancellationToken token = default)
    {
        if (isInitialized)
        {
            return;
        }

        using (ValueStopwatch.MeasureExecution(logger))
        {
            isInitialized = await DownloadMetadataDescriptionFileAndCheckAsync(token).ConfigureAwait(false);
            initializeCompletionSource.TrySetResult();
        }
    }

    public async ValueTask<T> FromCacheOrFileAsync<T>(string fileName, CancellationToken token)
        where T : class
    {
        Verify.Operation(isInitialized, SH.ServiceMetadataNotInitialized);
        string cacheKey = $"{nameof(MetadataService)}.Cache.{fileName}";

        if (memoryCache.TryGetValue(cacheKey, out object? value))
        {
            ArgumentNullException.ThrowIfNull(value);
            return (T)value;
        }

        string path = metadataOptions.GetLocalizedLocalFile($"{fileName}.json");
        if (File.Exists(path))
        {
            using (Stream fileStream = File.OpenRead(path))
            {
                T? result = await JsonSerializer.DeserializeAsync<T>(fileStream, options, token).ConfigureAwait(false);
                ArgumentNullException.ThrowIfNull(result);
                return memoryCache.Set(cacheKey, result);
            }
        }
        else
        {
            FileNotFoundException exception = new(SH.ServiceMetadataFileNotFound, fileName);
            throw HutaoException.Throw(SH.ServiceMetadataFileNotFound, exception);
        }
    }

    private async ValueTask<bool> DownloadMetadataDescriptionFileAndCheckAsync(CancellationToken token)
    {
        if (LocalSetting.Get(SettingKeys.SuppressMetadataInitialization, false))
        {
            return true;
        }

        if (await DownloadMetadataDescriptionFileAsync(token).ConfigureAwait(false) is not { } metadataFileHashs)
        {
            return false;
        }

        await CheckMetadataSourceFilesAsync(metadataFileHashs, token).ConfigureAwait(false);

        // save metadataFile
        using (FileStream metaFileStream = File.Create(metadataOptions.GetLocalizedLocalFile(MetaFileName)))
        {
            await JsonSerializer
                .SerializeAsync(metaFileStream, metadataFileHashs, options, token)
                .ConfigureAwait(false);
        }

        return true;
    }

    private async ValueTask<Dictionary<string, string>?> DownloadMetadataDescriptionFileAsync(CancellationToken token)
    {
        Dictionary<string, string>? metadataFileHashs;
        try
        {
            using (IServiceScope scope = serviceScopeFactory.CreateScope())
            {
                IHttpClientFactory httpClientFactory = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>();
                using (HttpClient httpClient = httpClientFactory.CreateClient(nameof(MetadataService)))
                {
                    // Download meta check file
                    metadataFileHashs = await httpClient
                        .GetFromJsonAsync<Dictionary<string, string>>(metadataOptions.GetLocalizedRemoteFile(MetaFileName), options, token)
                        .ConfigureAwait(false);
                }
            }

            if (metadataFileHashs is null)
            {
                infoBarService.Error(SH.ServiceMetadataParseFailed);
                return default;
            }

            return metadataFileHashs;
        }
        catch (JsonException ex)
        {
            infoBarService.Error(ex, SH.ServiceMetadataRequestFailed);
            return default;
        }
        catch (HttpRequestException ex)
        {
            if (ex.StatusCode is (HttpStatusCode)418)
            {
                infoBarService.Error(SH.ServiceMetadataVersionNotSupported);
            }
            else
            {
                infoBarService.Error(SH.FormatServiceMetadataHttpRequestFailed(ex.StatusCode, ex.HttpRequestError));
            }

            return default;
        }
    }

    private ValueTask CheckMetadataSourceFilesAsync(Dictionary<string, string> metaMd5Map, CancellationToken token)
    {
        return Parallel.ForEachAsync(metaMd5Map, token, async (pair, token) =>
        {
            (string fileName, string md5) = pair;
            string fileFullName = $"{fileName}.json";
            string fileFullPath = metadataOptions.GetLocalizedLocalFile(fileFullName);

            bool skip = false;
            if (File.Exists(fileFullPath))
            {
                skip = md5 == await XXH64.HashFileAsync(fileFullPath, token).ConfigureAwait(false);
            }

            if (!skip)
            {
                logger.LogInformation("{Hash} of {File} not matched, begin downloading", nameof(XXH64), fileFullName);
                await DownloadMetadataSourceFilesAsync(fileFullName, token).ConfigureAwait(false);
            }
        }).AsValueTask();
    }

    private async ValueTask DownloadMetadataSourceFilesAsync(string fileFullName, CancellationToken token)
    {
        Stream sourceStream;
        using (IServiceScope scope = serviceScopeFactory.CreateScope())
        {
            IHttpClientFactory httpClientFactory = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>();
            using (HttpClient httpClient = httpClientFactory.CreateClient(nameof(MetadataService)))
            {
                sourceStream = await httpClient
                    .GetStreamAsync(metadataOptions.GetLocalizedRemoteFile(fileFullName), token)
                    .ConfigureAwait(false);
            }
        }

        // Write stream while convert LF to CRLF
        using (StreamReaderWriter readerWriter = new(new(sourceStream), File.CreateText(metadataOptions.GetLocalizedLocalFile(fileFullName))))
        {
            while (await readerWriter.ReadLineAsync(token).ConfigureAwait(false) is { } line)
            {
                await readerWriter.WriteAsync(line).ConfigureAwait(false);

                if (!readerWriter.Reader.EndOfStream)
                {
                    await readerWriter.WriteAsync("\r\n").ConfigureAwait(false);
                }
            }
        }

        logger.LogInformation("Download {file} completed", fileFullName);
    }
}