// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Extensions.Caching.Memory;
using Snap.Hutao.Context.FileSystem;
using Snap.Hutao.Core.Abstraction;
using Snap.Hutao.Model.Metadata.Achievement;
using Snap.Hutao.Model.Metadata.Avatar;
using Snap.Hutao.Model.Metadata.Reliquary;
using Snap.Hutao.Model.Metadata.Weapon;
using Snap.Hutao.Service.Abstraction;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Cryptography;

namespace Snap.Hutao.Service.Metadata;

/// <summary>
/// 元数据服务
/// </summary>
[Injection(InjectAs.Singleton, typeof(IMetadataService))]
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

    private bool isInitialized = false;

    /// <summary>
    /// 构造一个新的元数据服务
    /// </summary>
    /// <param name="infoBarService">信息条服务</param>
    /// <param name="httpClient">http客户端</param>
    /// <param name="metadataContext">我的文档上下文</param>
    /// <param name="options">json序列化选项</param>
    /// <param name="logger">日志器</param>
    /// <param name="memoryCache">内存缓存</param>
    public MetadataService(
        IInfoBarService infoBarService,
        HttpClient httpClient,
        MetadataContext metadataContext,
        JsonSerializerOptions options,
        ILogger<MetadataService> logger,
        IMemoryCache memoryCache)
    {
        this.infoBarService = infoBarService;
        this.httpClient = httpClient;
        this.metadataContext = metadataContext;
        this.options = options;
        this.logger = logger;
        this.memoryCache = memoryCache;
    }

    /// <inheritdoc/>
    public bool IsInitialized { get => isInitialized; private set => isInitialized = value; }

    /// <inheritdoc/>
    public async ValueTask<bool> InitializeAsync(CancellationToken token = default)
    {
        await initializeCompletionSource.Task;
        return IsInitialized;
    }

    /// <inheritdoc/>
    public async Task InitializeInternalAsync(CancellationToken token = default)
    {
        logger.LogInformation("元数据初始化开始");
        metadataContext.EnsureDirectory();

        IsInitialized = await UpdateMetadataAsync(token)
            .ConfigureAwait(false);

        initializeCompletionSource.SetResult();
        logger.LogInformation("元数据初始化完成");
    }

    /// <inheritdoc/>
    public async Task<bool> UpdateMetadataAsync(CancellationToken token = default)
    {
        IDictionary<string, string>? metaMd5Map = await httpClient
            .GetFromJsonAsync<IDictionary<string, string>>($"{MetaAPIHost}/{MetaFileName}", options, token)
            .ConfigureAwait(false);

        if (metaMd5Map is null)
        {
            infoBarService.Error("元数据校验文件解析失败");
            return false;
        }

        await CheckMetadataAsync(metaMd5Map, token).ConfigureAwait(false);

        using (FileStream metaFileStream = metadataContext.Create(MetaFileName))
        {
            await JsonSerializer
                .SerializeAsync(metaFileStream, metaMd5Map, options, token)
                .ConfigureAwait(false);
        }

        return true;
    }

    /// <inheritdoc/>
    public ValueTask<IEnumerable<AchievementGoal>> GetAchievementGoalsAsync(CancellationToken token = default)
    {
        return GetMetadataAsync<IEnumerable<AchievementGoal>>("AchievementGoal", token);
    }

    /// <inheritdoc/>
    public ValueTask<IEnumerable<Achievement>> GetAchievementsAsync(CancellationToken token = default)
    {
        return GetMetadataAsync<IEnumerable<Achievement>>("Achievement", token);
    }

    /// <inheritdoc/>
    public ValueTask<IEnumerable<Avatar>> GetAvatarsAsync(CancellationToken token = default)
    {
        return GetMetadataAsync<IEnumerable<Avatar>>("Avatar", token);
    }

    /// <inheritdoc/>
    public ValueTask<IEnumerable<Reliquary>> GetReliquariesAsync(CancellationToken token = default)
    {
        return GetMetadataAsync<IEnumerable<Reliquary>>("Reliquary", token);
    }

    /// <inheritdoc/>
    public ValueTask<IEnumerable<ReliquaryAffix>> GetReliquaryAffixesAsync(CancellationToken token = default)
    {
        return GetMetadataAsync<IEnumerable<ReliquaryAffix>>("ReliquaryAffix", token);
    }

    /// <inheritdoc/>
    public ValueTask<IEnumerable<ReliquaryAffixBase>> GetReliquaryMainAffixesAsync(CancellationToken token = default)
    {
        return GetMetadataAsync<IEnumerable<ReliquaryAffixBase>>("ReliquaryMainAffix", token);
    }

    /// <inheritdoc/>
    public ValueTask<IEnumerable<Weapon>> GetWeaponsAsync(CancellationToken token = default)
    {
        return GetMetadataAsync<IEnumerable<Weapon>>("Weapon", token);
    }

    private async ValueTask<T> GetMetadataAsync<T>(string fileName, CancellationToken token)
        where T : class
    {
        Verify.Operation(IsInitialized, "元数据服务尚未初始化，或初始化失败");
        string cacheKey = $"{nameof(MetadataService)}.Cache.{fileName}";

        if (memoryCache.TryGetValue(cacheKey, out object? value))
        {
            return Must.NotNull((value as T)!);
        }

        T? result = await JsonSerializer
            .DeserializeAsync<T>(metadataContext.OpenRead($"{fileName}.json"), options, token)
            .ConfigureAwait(false);

        return memoryCache.Set(cacheKey, Must.NotNull(result!));
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

    /// <summary>
    /// 检查元数据的Md5值是否匹配
    /// 如果不匹配则尝试下载
    /// </summary>
    /// <param name="metaMd5Map">元数据校验表</param>
    /// <param name="token">取消令牌</param>
    /// <returns>令牌</returns>
    private async Task CheckMetadataAsync(IDictionary<string, string> metaMd5Map, CancellationToken token)
    {
        // enumerate files and compare md5
        foreach ((string fileName, string md5) in metaMd5Map)
        {
            string fileFullName = $"{fileName}.json";
            bool skip = false;

            if (metadataContext.FileExists(fileFullName))
            {
                skip = md5 == await GetFileMd5Async(fileFullName, token)
                    .ConfigureAwait(false);
            }

            if (!skip)
            {
                logger.LogInformation("{file} 文件 MD5 不匹配", fileFullName);

                await DownloadMetadataAsync(fileFullName, token)
                    .ConfigureAwait(false);
            }
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
                    await (streamReader.EndOfStream
                        ? streamWriter.WriteAsync(line) // Don't append the last line
                        : streamWriter.WriteLineAsync(line))
                        .ConfigureAwait(false);
                }
            }
        }

        logger.LogInformation("{file} 下载完成", fileFullName);
    }
}