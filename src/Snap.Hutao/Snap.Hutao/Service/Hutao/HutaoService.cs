// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Extensions.Caching.Memory;
using Snap.Hutao.Core.Database;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Entity.Database;
using Snap.Hutao.Web.Hutao;
using Snap.Hutao.Web.Hutao.Model;
using Snap.Hutao.Web.Response;

namespace Snap.Hutao.Service.Hutao;

/// <summary>
/// 胡桃 API 服务
/// </summary>
[HighQuality]
[Injection(InjectAs.Scoped, typeof(IHutaoService))]
internal sealed class HutaoService : IHutaoService
{
    private static readonly TimeSpan CacheExpireTime = TimeSpan.FromHours(4);
    private readonly HomaSpiralAbyssClient homaClient;
    private readonly IMemoryCache memoryCache;
    private readonly JsonSerializerOptions options;
    private readonly IServiceProvider serviceProvider;

    /// <summary>
    /// 构造一个新的胡桃 API 服务
    /// </summary>
    /// <param name="serviceProvider">服务提供器</param>
    public HutaoService(IServiceProvider serviceProvider)
    {
        homaClient = serviceProvider.GetRequiredService<HomaSpiralAbyssClient>();
        memoryCache = serviceProvider.GetRequiredService<IMemoryCache>();
        options = serviceProvider.GetRequiredService<JsonSerializerOptions>();

        this.serviceProvider = serviceProvider;
    }

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

    private async ValueTask<T> FromCacheOrWebAsync<T>(string typeName, Func<CancellationToken, Task<Response<T>>> taskFunc)
        where T : new()
    {
        string key = $"{nameof(HutaoService)}.Cache.{typeName}";
        if (memoryCache.TryGetValue(key, out object? cache))
        {
            return (T)cache!;
        }
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            if (appDbContext.ObjectCache.SingleOrDefault(e => e.Key == key) is ObjectCacheEntry entry)
            {
                if (entry.IsExpired)
                {
                    await appDbContext.ObjectCache.RemoveAndSaveAsync(entry).ConfigureAwait(false);
                }
                else
                {
                    T value = JsonSerializer.Deserialize<T>(entry.Value!, options)!;
                    return memoryCache.Set(key, value, TimeSpan.FromMinutes(30));
                }
            }
        }

        Response<T> webResponse = await taskFunc(default).ConfigureAwait(false);
        T? data = webResponse.Data;

        try
        {
            if (data != null)
            {
                using (IServiceScope scope = serviceProvider.CreateScope())
                {
                    AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    appDbContext.ObjectCache.AddAndSave(new()
                    {
                        Key = key,

                        // We hold the cache for 4 hours
                        ExpireTime = DateTimeOffset.Now.Add(CacheExpireTime),
                        Value = JsonSerializer.Serialize(data, options),
                    });
                }
            }
        }
        catch (Microsoft.EntityFrameworkCore.DbUpdateException)
        {
            // DbUpdateException: An error occurred while saving the entity changes.
            // TODO: Not ignore it.
        }

        return memoryCache.Set(key, data ?? new(), CacheExpireTime);
    }
}