// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Extensions.Caching.Memory;
using Snap.Hutao.Web.Hutao;
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
    private readonly TimeSpan cacheExpireTime = TimeSpan.FromHours(4);

    private readonly IObjectCacheDbService objectCacheDbService;
    private readonly HomaSpiralAbyssClient homaClient;
    private readonly JsonSerializerOptions options;
    private readonly IMemoryCache memoryCache;

    /// <inheritdoc/>
    public ValueTask<Overview> GetOverviewAsync()
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
    public ValueTask<List<WeaponCollocation>> GetWeaponCollocationsAsync()
    {
        return FromCacheOrWebAsync(nameof(WeaponCollocation), homaClient.GetWeaponCollocationsAsync);
    }

    /// <inheritdoc/>
    public ValueTask<List<TeamAppearance>> GetTeamAppearancesAsync()
    {
        return FromCacheOrWebAsync(nameof(TeamAppearance), homaClient.GetTeamCombinationsAsync);
    }

    private async ValueTask<T> FromCacheOrWebAsync<T>(string typeName, Func<CancellationToken, ValueTask<HutaoResponse<T>>> taskFunc)
        where T : class, new()
    {
        string key = $"{nameof(HutaoSpiralAbyssService)}.Cache.{typeName}";
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

        Response<T> webResponse = await taskFunc(default).ConfigureAwait(false);
        T? data = webResponse.Data;

        if (data is not null)
        {
            await objectCacheDbService.AddObjectCacheAsync(key, cacheExpireTime, data).ConfigureAwait(false);
        }

        return memoryCache.Set(key, data ?? new(), cacheExpireTime);
    }
}