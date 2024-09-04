// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Extensions.Caching.Memory;
using Snap.Hutao.Web.Hutao.Response;
using Snap.Hutao.Web.Hutao.SpiralAbyss;
using Snap.Hutao.Web.Response;

namespace Snap.Hutao.Service.Hutao;

[ConstructorGenerated]
[Injection(InjectAs.Scoped, typeof(IHutaoSpiralAbyssService))]
internal sealed partial class HutaoSpiralAbyssService : IHutaoSpiralAbyssService
{
    private readonly TimeSpan cacheExpireTime = TimeSpan.FromHours(1);

    private readonly IObjectCacheRepository objectCacheRepository;
    private readonly IServiceProvider serviceProvider;
    private readonly JsonSerializerOptions options;
    private readonly IMemoryCache memoryCache;

    public async ValueTask<Overview> GetOverviewAsync(bool last = false)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            HutaoSpiralAbyssClient homaClient = scope.ServiceProvider.GetRequiredService<HutaoSpiralAbyssClient>();
            return await FromCacheOrWebAsync(nameof(Overview), last, homaClient.GetOverviewAsync).ConfigureAwait(false);
        }
    }

    public async ValueTask<List<AvatarAppearanceRank>> GetAvatarAppearanceRanksAsync(bool last = false)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            HutaoSpiralAbyssClient homaClient = scope.ServiceProvider.GetRequiredService<HutaoSpiralAbyssClient>();
            return await FromCacheOrWebAsync(nameof(AvatarAppearanceRank), last, homaClient.GetAvatarAttendanceRatesAsync).ConfigureAwait(false);
        }
    }

    public async ValueTask<List<AvatarUsageRank>> GetAvatarUsageRanksAsync(bool last = false)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            HutaoSpiralAbyssClient homaClient = scope.ServiceProvider.GetRequiredService<HutaoSpiralAbyssClient>();
            return await FromCacheOrWebAsync(nameof(AvatarUsageRank), last, homaClient.GetAvatarUtilizationRatesAsync).ConfigureAwait(false);
        }
    }

    public async ValueTask<List<AvatarConstellationInfo>> GetAvatarConstellationInfosAsync(bool last = false)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            HutaoSpiralAbyssClient homaClient = scope.ServiceProvider.GetRequiredService<HutaoSpiralAbyssClient>();
            return await FromCacheOrWebAsync(nameof(AvatarConstellationInfo), last, homaClient.GetAvatarHoldingRatesAsync).ConfigureAwait(false);
        }
    }

    public async ValueTask<List<AvatarCollocation>> GetAvatarCollocationsAsync(bool last = false)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            HutaoSpiralAbyssClient homaClient = scope.ServiceProvider.GetRequiredService<HutaoSpiralAbyssClient>();
            return await FromCacheOrWebAsync(nameof(AvatarCollocation), last, homaClient.GetAvatarCollocationsAsync).ConfigureAwait(false);
        }
    }

    public async ValueTask<List<WeaponCollocation>> GetWeaponCollocationsAsync(bool last = false)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            HutaoSpiralAbyssClient homaClient = scope.ServiceProvider.GetRequiredService<HutaoSpiralAbyssClient>();
            return await FromCacheOrWebAsync(nameof(WeaponCollocation), last, homaClient.GetWeaponCollocationsAsync).ConfigureAwait(false);
        }
    }

    public async ValueTask<List<TeamAppearance>> GetTeamAppearancesAsync(bool last = false)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            HutaoSpiralAbyssClient homaClient = scope.ServiceProvider.GetRequiredService<HutaoSpiralAbyssClient>();
            return await FromCacheOrWebAsync(nameof(TeamAppearance), last, homaClient.GetTeamCombinationsAsync).ConfigureAwait(false);
        }
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

        if (await objectCacheRepository.GetObjectOrDefaultAsync<T>(key).ConfigureAwait(false) is { } value)
        {
            return memoryCache.Set(key, value, cacheExpireTime);
        }

        Response<T> webResponse = await taskFunc(last, default).ConfigureAwait(false);
        T? data = webResponse.Data;

        if (data is not null)
        {
            await objectCacheRepository.AddObjectCacheAsync(key, cacheExpireTime, data).ConfigureAwait(false);
        }

        return memoryCache.Set(key, data ?? new(), cacheExpireTime);
    }
}