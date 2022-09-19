// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Extensions.Caching.Memory;
using Snap.Hutao.Context.FileSystem;
using Snap.Hutao.Core.Abstraction;
using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Core.Diagnostics;
using Snap.Hutao.Core.Logging;
using Snap.Hutao.Extension;
using Snap.Hutao.Model.Metadata;
using Snap.Hutao.Model.Metadata.Achievement;
using Snap.Hutao.Model.Metadata.Avatar;
using Snap.Hutao.Model.Metadata.Reliquary;
using Snap.Hutao.Model.Metadata.Weapon;
using Snap.Hutao.Service.Abstraction;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Cryptography;

namespace Snap.Hutao.Service.Metadata;

/// <summary>
/// 元数据服务
/// </summary>
[Injection(InjectAs.Singleton, typeof(IMetadataService))]
[HttpClient(HttpClientConfigration.Default)]
internal class MetadataService : IMetadataService, IMetadataInitializer, ISupportAsyncInitialization
{
    private const string MetaAPIHost = "http://hutao-metadata.snapgenshin.com";
    private const string MetaFileName = "Meta.json";

    private readonly IInfoBarService infoBarService;
    private readonly HttpClient httpClient;
    private readonly FileSystemContext metadataContext;
    private readonly JsonSerializerOptions options;
    private readonly ILogger<MetadataService> logger;
    private readonly IMemoryCache memoryCache;

    /// <summary>
    /// 用于指示初始化是否完成
    /// </summary>
    private readonly TaskCompletionSource initializeCompletionSource = new();

    private bool isInitialized;

    /// <summary>
    /// 构造一个新的元数据服务
    /// </summary>
    /// <param name="infoBarService">信息条服务</param>
    /// <param name="httpClientFactory">http客户端工厂</param>
    /// <param name="metadataContext">我的文档上下文</param>
    /// <param name="options">json序列化选项</param>
    /// <param name="logger">日志器</param>
    /// <param name="memoryCache">内存缓存</param>
    public MetadataService(
        IInfoBarService infoBarService,
        IHttpClientFactory httpClientFactory,
        MetadataContext metadataContext,
        JsonSerializerOptions options,
        ILogger<MetadataService> logger,
        IMemoryCache memoryCache)
    {
        this.infoBarService = infoBarService;
        this.metadataContext = metadataContext;
        this.options = options;
        this.logger = logger;
        this.memoryCache = memoryCache;
        httpClient = httpClientFactory.CreateClient(nameof(MetadataService));
    }

    /// <inheritdoc/>
    public bool IsInitialized { get => isInitialized; private set => isInitialized = value; }

    /// <inheritdoc/>
    public async ValueTask<bool> InitializeAsync(CancellationToken token = default)
    {
        await initializeCompletionSource.Task.ConfigureAwait(false);
        return IsInitialized;
    }

    /// <inheritdoc/>
    public async Task InitializeInternalAsync(CancellationToken token = default)
    {
        ValueStopwatch stopwatch = ValueStopwatch.StartNew();
        logger.LogInformation(EventIds.MetadataInitialization, "Metadata initializaion begin");

        IsInitialized = await TryUpdateMetadataAsync(token).ConfigureAwait(false);
        initializeCompletionSource.SetResult();

        logger.LogInformation(EventIds.MetadataInitialization, "Metadata initializaion completed in {time}ms", stopwatch.GetElapsedTime().TotalMilliseconds);
    }

    /// <inheritdoc/>
    public ValueTask<List<AchievementGoal>> GetAchievementGoalsAsync(CancellationToken token = default)
    {
        return FromCacheOrFileAsync<List<AchievementGoal>>("AchievementGoal", token);
    }

    /// <inheritdoc/>
    public ValueTask<List<Model.Metadata.Achievement.Achievement>> GetAchievementsAsync(CancellationToken token = default)
    {
        return FromCacheOrFileAsync<List<Model.Metadata.Achievement.Achievement>>("Achievement", token);
    }

    /// <inheritdoc/>
    public ValueTask<List<Avatar>> GetAvatarsAsync(CancellationToken token = default)
    {
        return FromCacheOrFileAsync<List<Avatar>>("Avatar", token);
    }

    /// <inheritdoc/>
    public ValueTask<List<GachaEvent>> GetGachaEventsAsync(CancellationToken token = default)
    {
        return FromCacheOrFileAsync<List<GachaEvent>>("GachaEvent", token);
    }

    /// <inheritdoc/>
    public ValueTask<Dictionary<int, Avatar>> GetIdToAvatarMapAsync(CancellationToken token = default)
    {
        return FromCacheAsDictionaryAsync<int, Avatar>("Avatar", a => a.Id, token);
    }

    /// <inheritdoc/>
    public ValueTask<Dictionary<int, Weapon>> GetIdToWeaponMapAsync(CancellationToken token = default)
    {
        return FromCacheAsDictionaryAsync<int, Weapon>("Weapon", w => w.Id, token);
    }

    /// <inheritdoc/>
    public ValueTask<Dictionary<string, Avatar>> GetNameToAvatarMapAsync(CancellationToken token = default)
    {
        return FromCacheAsDictionaryAsync<string, Avatar>("Avatar", a => a.Name, token);
    }

    /// <inheritdoc/>
    public ValueTask<Dictionary<string, Weapon>> GetNameToWeaponMapAsync(CancellationToken token = default)
    {
        return FromCacheAsDictionaryAsync<string, Weapon>("Weapon", w => w.Name, token);
    }

    /// <inheritdoc/>
    public ValueTask<List<Reliquary>> GetReliquariesAsync(CancellationToken token = default)
    {
        return FromCacheOrFileAsync<List<Reliquary>>("Reliquary", token);
    }

    /// <inheritdoc/>
    public ValueTask<List<ReliquaryAffix>> GetReliquaryAffixesAsync(CancellationToken token = default)
    {
        return FromCacheOrFileAsync<List<ReliquaryAffix>>("ReliquaryAffix", token);
    }

    /// <inheritdoc/>
    public ValueTask<List<ReliquaryAffixBase>> GetReliquaryMainAffixesAsync(CancellationToken token = default)
    {
        return FromCacheOrFileAsync<List<ReliquaryAffixBase>>("ReliquaryMainAffix", token);
    }

    /// <inheritdoc/>
    public ValueTask<List<Weapon>> GetWeaponsAsync(CancellationToken token = default)
    {
        return FromCacheOrFileAsync<List<Weapon>>("Weapon", token);
    }

    private async Task<bool> TryUpdateMetadataAsync(CancellationToken token)
    {
        IDictionary<string, string>? metaMd5Map = null;
        try
        {
            // download meta check file
            metaMd5Map = await httpClient
                .GetFromJsonAsync<IDictionary<string, string>>($"{MetaAPIHost}/{MetaFileName}", options, token)
                .ConfigureAwait(false);

            if (metaMd5Map is null)
            {
                infoBarService.Error("元数据校验文件解析失败");
                return false;
            }
        }
        catch (HttpRequestException ex)
        {
            infoBarService.Error(ex, "元数据校验文件下载失败");
            return false;
        }

        await CheckMetadataAsync(metaMd5Map, token).ConfigureAwait(false);

        // save metadataFile
        using (FileStream metaFileStream = metadataContext.Create(MetaFileName))
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
            if (metadataContext.FileExists(fileFullName))
            {
                skip = md5 == await GetFileMd5Async(fileFullName, token).ConfigureAwait(false);
            }

            if (!skip)
            {
                logger.LogInformation(EventIds.MetadataFileMD5Check, "MD5 of {file} not matched, begin downloading", fileFullName);

                await DownloadMetadataAsync(fileFullName, token).ConfigureAwait(false);
            }
        });
    }

    private async Task<string> GetFileMd5Async(string fileFullName, CancellationToken token)
    {
        using (FileStream stream = metadataContext.OpenRead(fileFullName))
        {
            byte[] bytes = await MD5.Create()
                .ComputeHashAsync(stream, token)
                .ConfigureAwait(false);

            return Convert.ToHexString(bytes);
        }
    }

    private async Task DownloadMetadataAsync(string fileFullName, CancellationToken token)
    {
        Stream sourceStream = await httpClient
            .GetStreamAsync($"{MetaAPIHost}/{fileFullName}", token)
            .ConfigureAwait(false);

        // Write stream while convert LF to CRLF
        using (StreamReader streamReader = new(sourceStream))
        {
            using (StreamWriter streamWriter = new(metadataContext.Create(fileFullName)))
            {
                while (await streamReader.ReadLineAsync().ConfigureAwait(false) is string line)
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
        Verify.Operation(IsInitialized, "元数据服务尚未初始化，或初始化失败");
        string cacheKey = $"{nameof(MetadataService)}.Cache.{fileName}";

        if (memoryCache.TryGetValue(cacheKey, out object? value))
        {
            return Must.NotNull((T)value!);
        }

        using (Stream fileStream = metadataContext.OpenRead($"{fileName}.json"))
        {
            T? result = await JsonSerializer.DeserializeAsync<T>(fileStream, options, token).ConfigureAwait(false);
            return memoryCache.Set(cacheKey, Must.NotNull(result!));
        }
    }

    private async ValueTask<Dictionary<TKey, TValue>> FromCacheAsDictionaryAsync<TKey, TValue>(string fileName, Func<TValue, TKey> keySelector, CancellationToken token)
        where TKey : notnull
    {
        Verify.Operation(IsInitialized, "元数据服务尚未初始化，或初始化失败");
        string cacheKey = $"{nameof(MetadataService)}.Cache.{fileName}.Map.{typeof(TKey).Name}";

        if (memoryCache.TryGetValue(cacheKey, out object? value))
        {
            return Must.NotNull((Dictionary<TKey, TValue>)value!);
        }

        List<TValue> list = await FromCacheOrFileAsync<List<TValue>>(fileName, token).ConfigureAwait(false);
        Dictionary<TKey, TValue> dict = list.ToDictionaryOverride(keySelector);
        return memoryCache.Set(cacheKey, dict);
    }
}