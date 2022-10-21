// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Extensions.Caching.Memory;
using Snap.Hutao.Service.Abstraction;
using Snap.Hutao.Web.Hutao;
using Snap.Hutao.Web.Hutao.Model;

namespace Snap.Hutao.Service;

/// <summary>
/// 胡桃 API 服务
/// </summary>
[Injection(InjectAs.Transient, typeof(IHutaoService))]
internal class HutaoService : IHutaoService
{
    private readonly HomaClient homaClient;
    private readonly IMemoryCache memoryCache;

    /// <summary>
    /// 构造一个新的胡桃 API 服务
    /// </summary>
    /// <param name="homaClient">胡桃 API 客户端</param>
    /// <param name="memoryCache">内存缓存</param>
    public HutaoService(HomaClient homaClient, IMemoryCache memoryCache)
    {
        this.homaClient = homaClient;
        this.memoryCache = memoryCache;
    }

    /// <inheritdoc/>
    public ValueTask<Overview?> GetOverviewAsync()
    {
        return FromCacheOrWebAsync(nameof(Overview), homaClient.GetOverviewAsync);
    }

    /// <inheritdoc/>
    public ValueTask<List<AvatarAppearanceRank>> GetAvatarAppearanceRanksAsync()
    {
        return FromCacheOrWebAsync(nameof(AvatarAppearanceRank), homaClient.GetAvatarAttendanceRatesAsync);
    }

    /// <inheritdoc/>
    public ValueTask<List<AvatarUsageRank>> GetAvatarUsageRanksAsync()
    {
        return FromCacheOrWebAsync(nameof(AvatarUsageRank), homaClient.GetAvatarUtilizationRatesAsync);
    }

    /// <inheritdoc/>
    public ValueTask<List<AvatarConstellationInfo>> GetAvatarConstellationInfosAsync()
    {
        return FromCacheOrWebAsync(nameof(AvatarConstellationInfo), homaClient.GetAvatarHoldingRatesAsync);
    }

    /// <inheritdoc/>
    public ValueTask<List<AvatarCollocation>> GetAvatarCollocationsAsync()
    {
        return FromCacheOrWebAsync(nameof(AvatarCollocation), homaClient.GetAvatarCollocationsAsync);
    }

    /// <inheritdoc/>
    public ValueTask<List<TeamAppearance>> GetTeamAppearancesAsync()
    {
        return FromCacheOrWebAsync(nameof(TeamAppearance), homaClient.GetTeamCombinationsAsync);
    }

    private async ValueTask<T> FromCacheOrWebAsync<T>(string typeName, Func<CancellationToken, Task<T>> taskFunc)
    {
        string key = $"{nameof(HutaoService)}.Cache.{typeName}";
        if (memoryCache.TryGetValue(key, out object? cache))
        {
            return (T)cache;
        }

        T web = await taskFunc(default).ConfigureAwait(false);
        return memoryCache.Set(key, web, TimeSpan.FromMinutes(30));
    }
}