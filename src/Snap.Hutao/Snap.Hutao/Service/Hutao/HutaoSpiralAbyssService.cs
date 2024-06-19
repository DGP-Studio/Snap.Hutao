// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Extensions.Caching.Memory;
using Snap.Hutao.Web.Hutao.Response;
using Snap.Hutao.Web.Hutao.SpiralAbyss;
using Snap.Hutao.Web.Response;

namespace Snap.Hutao.Service.Hutao;

/// <summary>
/// 胡桃 API 服务
/// </summary>
[HighQuality]
[ConstructorGenerated]
[Injection(InjectAs.Scoped, typeof(IHutaoSpiralAbyssService))]
internal sealed partial class HutaoSpiralAbyssService : IHutaoSpiralAbyssService
{
    private readonly TimeSpan cacheExpireTime = TimeSpan.FromHours(1);

    private readonly IObjectCacheDbService objectCacheDbService;
    private readonly HutaoSpiralAbyssClient homaClient;
    private readonly JsonSerializerOptions options;
    private readonly IMemoryCache memoryCache;

    /// <inheritdoc/>
    public ValueTask<Overview> GetOverviewAsync(bool last = false)
    {
        return FromCacheOrWebAsync(nameof(Overview), last, homaClient.GetOverviewAsync);
    }

    /// <inheritdoc/>
    public ValueTask<List<AvatarAppearanceRank>> GetAvatarAppearanceRanksAsync(bool last = false)
    {
        return FromCacheOrWebAsync(nameof(AvatarAppearanceRank), last, homaClient.GetAvatarAttendanceRatesAsync);
    }

    /// <inheritdoc/>
    public ValueTask<List<AvatarUsageRank>> GetAvatarUsageRanksAsync(bool last = false)
    {
        return FromCacheOrWebAsync(nameof(AvatarUsageRank), last, homaClient.GetAvatarUtilizationRatesAsync);
    }

    /// <inheritdoc/>
    public ValueTask<List<AvatarConstellationInfo>> GetAvatarConstellationInfosAsync(bool last = false)
    {
        return FromCacheOrWebAsync(nameof(AvatarConstellationInfo), last, homaClient.GetAvatarHoldingRatesAsync);
    }

    /// <inheritdoc/>
    public ValueTask<List<AvatarCollocation>> GetAvatarCollocationsAsync(bool last = false)
    {
        return FromCacheOrWebAsync(nameof(AvatarCollocation), last, homaClient.GetAvatarCollocationsAsync);
    }

    /// <inheritdoc/>
    public ValueTask<List<WeaponCollocation>> GetWeaponCollocationsAsync(bool last = false)
    {
        return FromCacheOrWebAsync(nameof(WeaponCollocation), last, homaClient.GetWeaponCollocationsAsync);
    }

    /// <inheritdoc/>
    public ValueTask<List<TeamAppearance>> GetTeamAppearancesAsync(bool last = false)
    {
        return FromCacheOrWebAsync(nameof(TeamAppearance), last, homaClient.GetTeamCombinationsAsync);
    }

    private async ValueTask<T> FromCacheOrWebAsync<T>(string typeName, bool last, Func<bool, CancellationToken, ValueTask<HutaoResponse<T>>> taskFunc)
        where T : class, new()
    {
        string kind = last ? "Last" : "Current";
        string key = $"{nameof(HutaoSpiralAbyssService)}.Cache.{typeName}.{kind}";
        if (memoryCache.TryGetValue(key, out object? cache))
        {
            T? t = cache as T;
            ArgumentNullException.ThrowIfNull(t);
            return t;
        }

        if (await objectCacheDbService.GetObjectOrDefaultAsync<T>(key).ConfigureAwait(false) is { } value)
        {
            return memoryCache.Set(key, value, cacheExpireTime);
        }

        Response<T> webResponse = await taskFunc(last, default).ConfigureAwait(false);
        T? data = webResponse.Data;

        if (data is not null)
        {
            await objectCacheDbService.AddObjectCacheAsync(key, cacheExpireTime, data).ConfigureAwait(false);
        }

        return memoryCache.Set(key, data ?? new(), cacheExpireTime);
    }
}