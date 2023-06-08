// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Extensions.Caching.Memory;
using Snap.Hutao.Core;
using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Core.Diagnostics;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Core.IO.Hashing;
using Snap.Hutao.Service.Notification;
using System.IO;
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

    private readonly TaskCompletionSource initializeCompletionSource;
    private readonly ILogger<MetadataService> logger;
    private readonly MetadataOptions metadataOptions;
    private readonly IInfoBarService infoBarService;
    private readonly JsonSerializerOptions options;
    private readonly IMemoryCache memoryCache;
    private readonly HttpClient httpClient;

    private bool isInitialized;

    /// <inheritdoc/>
    public async ValueTask<bool> InitializeAsync()
    {
        await initializeCompletionSource.Task.ConfigureAwait(false);
        return isInitialized;
    }

    /// <inheritdoc/>
    public async Task InitializeInternalAsync(CancellationToken token = default)
    {
        if (isInitialized)
        {
            return;
        }

        using (ValueStopwatch.MeasureExecution(logger))
        {
            isInitialized = await TryUpdateMetadataAsync(token).ConfigureAwait(false);
            initializeCompletionSource.TrySetResult();
        }
    }

    private async Task<bool> TryUpdateMetadataAsync(CancellationToken token)
    {
        IDictionary<string, string>? metaMd5Map;
        try
        {
            string metadataFile = metadataOptions.GetLocalizedRemoteFile(MetaFileName);

            // download meta check file
            metaMd5Map = await httpClient
                .GetFromJsonAsync<IDictionary<string, string>>(metadataFile, options, token)
                .ConfigureAwait(false);

            if (metaMd5Map is null)
            {
                infoBarService.Error(SH.ServiceMetadataParseFailed);
                return false;
            }
        }
        catch (HttpRequestException ex)
        {
            if (ex.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                infoBarService.Error(SH.ServiceMetadataVersionNotSupported);
            }
            else
            {
                infoBarService.Error(ex, SH.ServiceMetadataRequestFailed);
            }

            return false;
        }

        await CheckMetadataAsync(metaMd5Map, token).ConfigureAwait(false);

        // save metadataFile
        using (FileStream metaFileStream = File.Create(metadataOptions.GetLocalizedLocalFile(MetaFileName)))
        {
            await JsonSerializer
                .SerializeAsync(metaFileStream, metaMd5Map, options, token)
                .ConfigureAwait(false);
        }

        return true;
    }

    /// <summary>
    /// 检查元数据的Md5值是否匹配
    /// 如果不匹配则尝试下载
    /// </summary>
    /// <param name="metaMd5Map">元数据校验表</param>
    /// <param name="token">取消令牌</param>
    /// <returns>令牌</returns>
    private Task CheckMetadataAsync(IDictionary<string, string> metaMd5Map, CancellationToken token)
    {
        return Parallel.ForEachAsync(metaMd5Map, token, async (pair, token) =>
        {
            (string fileName, string md5) = pair;
            string fileFullName = $"{fileName}.json";

            bool skip = false;
            string fileFullPath = metadataOptions.GetLocalizedLocalFile(fileFullName);
            if (File.Exists(fileFullPath))
            {
                skip = md5 == await XXH64.HashFileAsync(fileFullPath, token).ConfigureAwait(false);
            }

            if (!skip)
            {
                logger.LogInformation("{hash} of {file} not matched, begin downloading", nameof(XXH64), fileFullName);

                await DownloadMetadataAsync(fileFullName, token).ConfigureAwait(false);
            }
        });
    }

    private async Task DownloadMetadataAsync(string fileFullName, CancellationToken token)
    {
        Stream sourceStream = await httpClient
            .GetStreamAsync(metadataOptions.GetLocalizedRemoteFile(fileFullName), token)
            .ConfigureAwait(false);

        // Write stream while convert LF to CRLF
        using (StreamReader streamReader = new(sourceStream))
        {
            using (StreamWriter streamWriter = File.CreateText(metadataOptions.GetLocalizedLocalFile(fileFullName)))
            {
                while (await streamReader.ReadLineAsync(token).ConfigureAwait(false) is string line)
                {
                    await streamWriter.WriteAsync(line).ConfigureAwait(false);

                    if (!streamReader.EndOfStream)
                    {
                        await streamWriter.WriteAsync(StringLiterals.CRLF).ConfigureAwait(false);
                    }
                }
            }
        }

        logger.LogInformation("Download {file} completed", fileFullName);
    }

    private async ValueTask<T> FromCacheOrFileAsync<T>(string fileName, CancellationToken token)
        where T : class
    {
        Verify.Operation(isInitialized, SH.ServiceMetadataNotInitialized);
        string cacheKey = $"{nameof(MetadataService)}.Cache.{fileName}";

        if (memoryCache.TryGetValue(cacheKey, out object? value))
        {
            return Must.NotNull((T)value!);
        }

        string path = metadataOptions.GetLocalizedLocalFile($"{fileName}.json");
        if (File.Exists(path))
        {
            using (Stream fileStream = File.OpenRead(path))
            {
                T? result = await JsonSerializer.DeserializeAsync<T>(fileStream, options, token).ConfigureAwait(false);
                return memoryCache.Set(cacheKey, Must.NotNull(result!));
            }
        }
        else
        {
            FileNotFoundException exception = new(SH.ServiceMetadataFileNotFound, fileName);
            throw ThrowHelper.UserdataCorrupted(SH.ServiceMetadataFileNotFound, null!);
        }
    }

    private async ValueTask<Dictionary<TKey, TValue>> FromCacheAsDictionaryAsync<TKey, TValue>(string fileName, Func<TValue, TKey> keySelector, CancellationToken token)
        where TKey : notnull
    {
        string cacheKey = $"{nameof(MetadataService)}.Cache.{fileName}.Map.{typeof(TKey).Name}";

        if (memoryCache.TryGetValue(cacheKey, out object? value))
        {
            return Must.NotNull((Dictionary<TKey, TValue>)value!);
        }

        List<TValue> list = await FromCacheOrFileAsync<List<TValue>>(fileName, token).ConfigureAwait(false);
        Dictionary<TKey, TValue> dict = list.ToDictionaryOverride(keySelector);
        return memoryCache.Set(cacheKey, dict);
    }

    private async ValueTask<Dictionary<TKey, TValue>> FromCacheAsDictionaryAsync<TKey, TValue, TData>(string fileName, Func<TData, TKey> keySelector, Func<TData, TValue> valueSelector, CancellationToken token)
        where TKey : notnull
    {
        string cacheKey = $"{nameof(MetadataService)}.Cache.{fileName}.Map.{typeof(TKey).Name}.{typeof(TValue).Name}";

        if (memoryCache.TryGetValue(cacheKey, out object? value))
        {
            return Must.NotNull((Dictionary<TKey, TValue>)value!);
        }

        List<TData> list = await FromCacheOrFileAsync<List<TData>>(fileName, token).ConfigureAwait(false);
        Dictionary<TKey, TValue> dict = list.ToDictionaryOverride(keySelector, valueSelector);
        return memoryCache.Set(cacheKey, dict);
    }
}