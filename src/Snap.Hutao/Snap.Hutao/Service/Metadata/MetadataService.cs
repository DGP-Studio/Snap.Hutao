// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Extensions.Caching.Memory;
using Snap.Hutao.Core;
using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Core.Diagnostics;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Core.IO;
using Snap.Hutao.Service.Abstraction;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;

namespace Snap.Hutao.Service.Metadata;

/// <summary>
/// 元数据服务
/// </summary>
[HighQuality]
[Injection(InjectAs.Singleton, typeof(IMetadataService))]
[HttpClient(HttpClientConfiguration.Default)]
internal sealed partial class MetadataService : IMetadataService, IMetadataServiceInitialization
{
    private const string MetaFileName = "Meta.json";
    private readonly string metadataFolderPath;

    private readonly IInfoBarService infoBarService;
    private readonly HttpClient httpClient;
    private readonly JsonSerializerOptions options;
    private readonly ILogger<MetadataService> logger;
    private readonly IMemoryCache memoryCache;
    private readonly string locale;

    /// <summary>
    /// 用于指示初始化是否完成
    /// </summary>
    private readonly TaskCompletionSource initializeCompletionSource = new();

    private bool isInitialized;

    /// <summary>
    /// 构造一个新的元数据服务
    /// </summary>
    /// <param name="serviceProvider">服务提供器</param>
    /// <param name="httpClientFactory">http客户端工厂</param>
    public MetadataService(IServiceProvider serviceProvider, IHttpClientFactory httpClientFactory)
    {
        infoBarService = serviceProvider.GetRequiredService<IInfoBarService>();
        options = serviceProvider.GetRequiredService<JsonSerializerOptions>();
        logger = serviceProvider.GetRequiredService<ILogger<MetadataService>>();
        memoryCache = serviceProvider.GetRequiredService<IMemoryCache>();
        httpClient = httpClientFactory.CreateClient(nameof(MetadataService));

        locale = GetTextMapLocale();
        HutaoOptions hutaoOptions = serviceProvider.GetRequiredService<HutaoOptions>();
        metadataFolderPath = Path.Combine(hutaoOptions.DataFolder, "Metadata", locale);
        Directory.CreateDirectory(metadataFolderPath);
    }

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

        ValueStopwatch stopwatch = ValueStopwatch.StartNew();
        logger.LogInformation("Metadata initialization begin");

        isInitialized = await TryUpdateMetadataAsync(token).ConfigureAwait(false);
        initializeCompletionSource.TrySetResult();

        logger.LogInformation("Metadata initialization completed in {time}ms", stopwatch.GetElapsedTime().TotalMilliseconds);
    }

    private static string GetTextMapLocale()
    {
        CultureInfo cultureInfo = CultureInfo.CurrentCulture;

        while (true)
        {
            if (cultureInfo == CultureInfo.InvariantCulture)
            {
                // Fallback to Chinese.
                return "CHS";
            }

            switch (cultureInfo.Name)
            {
                case "de": return "DE";         // German
                case "en": return "EN";         // English
                case "es": return "ES";         // Spanish
                case "fr": return "FR";         // French
                case "id": return "ID";         // Indonesian
                case "it": return "IT";         // Italian
                case "ja": return "JP";         // Japanese
                case "ko": return "KR";         // Korean
                case "pt": return "PT";         // Portuguese
                case "ru": return "RU";         // Russian
                case "th": return "TH";         // Thai
                case "tr": return "TR";         // Turkish
                case "vi": return "TR";         // Vietnamese
                case "zh-CHS": return "CHS";    // Chinese (Simplified) Legacy
                case "zh-CHT": return "CHT";    // Chinese (Traditional) Legacy
                default: cultureInfo = cultureInfo.Parent; break;
            }
        }
    }

    private async Task<bool> TryUpdateMetadataAsync(CancellationToken token)
    {
        IDictionary<string, string>? metaMd5Map;
        try
        {
            // download meta check file
            metaMd5Map = await httpClient
                .GetFromJsonAsync<IDictionary<string, string>>(Web.HutaoEndpoints.HutaoMetadataFile(locale, MetaFileName), options, token)
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
        using (FileStream metaFileStream = File.Create(Path.Combine(metadataFolderPath, MetaFileName)))
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
            string fileFullPath = Path.Combine(metadataFolderPath, fileFullName);
            if (File.Exists(fileFullPath))
            {
                skip = md5 == await Digest.GetFileMD5Async(fileFullPath, token).ConfigureAwait(false);
            }

            if (!skip)
            {
                logger.LogInformation("MD5 of {file} not matched, begin downloading", fileFullName);

                await DownloadMetadataAsync(fileFullName, token).ConfigureAwait(false);
            }
        });
    }

    private async Task DownloadMetadataAsync(string fileFullName, CancellationToken token)
    {
        Stream sourceStream = await httpClient
            .GetStreamAsync(Web.HutaoEndpoints.HutaoMetadataFile(locale, fileFullName), token)
            .ConfigureAwait(false);

        // Write stream while convert LF to CRLF
        using (StreamReader streamReader = new(sourceStream))
        {
            using (StreamWriter streamWriter = new(File.Create(Path.Combine(metadataFolderPath, fileFullName))))
            {
                while (await streamReader.ReadLineAsync(token).ConfigureAwait(false) is string line)
                {
                    Func<string?, Task> write = streamReader.EndOfStream ? streamWriter.WriteAsync : streamWriter.WriteLineAsync;
                    await write(line).ConfigureAwait(false);
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

        string path = Path.Combine(metadataFolderPath, $"{fileName}.json");
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
            throw ThrowHelper.UserdataCorrupted(SH.ServiceMetadataNotInitialized, null!);
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