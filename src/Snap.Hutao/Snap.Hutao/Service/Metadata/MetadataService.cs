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

    public async ValueTask<List<T>> FromCacheOrFileAsync<T>(MetadataFileStrategy strategy, CancellationToken token)
        where T : class
    {
        Verify.Operation(isInitialized, SH.ServiceMetadataNotInitialized);
        string cacheKey = $"{nameof(MetadataService)}.Cache.{strategy.Name}";

        if (memoryCache.TryGetValue(cacheKey, out object? value))
        {
            ArgumentNullException.ThrowIfNull(value);
            return (List<T>)value;
        }

        return strategy.IsScattered
            ? await FromCacheOrScatteredFile<T>(strategy, cacheKey, token).ConfigureAwait(false)
            : await FromCacheOrSingleFile<T>(strategy, cacheKey, token).ConfigureAwait(false);
    }

    private async ValueTask<List<T>> FromCacheOrSingleFile<T>(MetadataFileStrategy strategy, string cacheKey, CancellationToken token)
        where T : class
    {
        string path = metadataOptions.GetLocalizedLocalPath($"{strategy.Name}.json");
        if (!File.Exists(path))
        {
            FileNotFoundException exception = new(SH.ServiceMetadataFileNotFound, strategy.Name);
            throw HutaoException.Throw(SH.ServiceMetadataFileNotFound, exception);
        }

        using (Stream fileStream = File.OpenRead(path))
        {
            List<T>? result = await JsonSerializer.DeserializeAsync<List<T>>(fileStream, options, token).ConfigureAwait(false);
            ArgumentNullException.ThrowIfNull(result);
            return memoryCache.Set(cacheKey, result);
        }
    }

    private async ValueTask<List<T>> FromCacheOrScatteredFile<T>(MetadataFileStrategy strategy, string cacheKey, CancellationToken token)
        where T : class
    {
        string path = metadataOptions.GetLocalizedLocalPath(strategy.Name);
        if (!Directory.Exists(path))
        {
            DirectoryNotFoundException exception = new(SH.ServiceMetadataFileNotFound);
            throw HutaoException.Throw(SH.ServiceMetadataFileNotFound, exception);
        }

        List<T> results = [];
        foreach (string file in Directory.GetFiles(path, "*.json"))
        {
            using (Stream fileStream = File.OpenRead(file))
            {
                T? result = await JsonSerializer.DeserializeAsync<T>(fileStream, options, token).ConfigureAwait(false);
                ArgumentNullException.ThrowIfNull(result);
                results.Add(result);
            }
        }

        return memoryCache.Set(cacheKey, results);
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
        using (FileStream metaFileStream = File.Create(metadataOptions.GetLocalizedLocalPath(MetaFileName)))
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

    [SuppressMessage("", "SH003")]
    private Task CheckMetadataSourceFilesAsync(Dictionary<string, string> metaHashMap, CancellationToken token)
    {
        return Parallel.ForEachAsync(metaHashMap, token, async (pair, token) =>
        {
            (string fileName, string hash) = pair;
            string fileFullName = $"{fileName}.json";
            string fileFullPath = metadataOptions.GetLocalizedLocalPath(fileFullName);
            if (Path.GetDirectoryName(fileFullPath) is { } directory && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            bool skip = false;
            if (File.Exists(fileFullPath))
            {
                skip = hash == await XXH64.HashFileAsync(fileFullPath, token).ConfigureAwait(false);
            }

            if (!skip)
            {
                logger.LogInformation("{Hash} of {File} not matched, begin downloading", nameof(XXH64), fileFullName);
                await DownloadMetadataSourceFilesAsync(fileFullName, token).ConfigureAwait(false);
            }
        });
    }

    private async ValueTask DownloadMetadataSourceFilesAsync(string fileFullName, CancellationToken token)
    {
        using (IServiceScope scope = serviceScopeFactory.CreateScope())
        {
            IHttpClientFactory httpClientFactory = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>();
            using (HttpClient httpClient = httpClientFactory.CreateClient(nameof(MetadataService)))
            {
                using (HttpRequestMessage message = new(HttpMethod.Get, metadataOptions.GetLocalizedRemoteFile(fileFullName)))
                {
                    // We have too much line endings now, should cache the response.
                    using (HttpResponseMessage responseMessage = await httpClient.SendAsync(message, HttpCompletionOption.ResponseContentRead, token).ConfigureAwait(false))
                    {
                        Stream sourceStream = await responseMessage.Content.ReadAsStreamAsync(token).ConfigureAwait(false);

                        // Write stream while convert LF to CRLF
                        using (StreamReaderWriter readerWriter = new(new(sourceStream), File.CreateText(metadataOptions.GetLocalizedLocalPath(fileFullName))))
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
                    }
                }
            }
        }

        logger.LogInformation("Download {file} completed", fileFullName);
    }
}