// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Snap.Hutao.Context.Database;
using Snap.Hutao.Core.Threading;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Game.Locator;

namespace Snap.Hutao.Service.Game;

/// <summary>
/// 游戏服务
/// </summary>
[Injection(InjectAs.Transient, typeof(IGameService))]
internal class GameService : IGameService
{
    private const string GamePath = "GamePath";

    private readonly AppDbContext appDbContext;
    private readonly IMemoryCache memoryCache;
    private readonly IEnumerable<IGameLocator> gameLocators;

    /// <summary>
    /// 构造一个新的游戏服务
    /// </summary>
    /// <param name="appDbContext">数据库上下文</param>
    /// <param name="memoryCache">内存缓存</param>
    /// <param name="gameLocators">游戏定位器集合</param>
    public GameService(AppDbContext appDbContext, IMemoryCache memoryCache, IEnumerable<IGameLocator> gameLocators)
    {
        this.appDbContext = appDbContext;
        this.memoryCache = memoryCache;
        this.gameLocators = gameLocators;
    }

    /// <inheritdoc/>
    public async ValueTask<ValueResult<bool, string>> GetGamePathAsync()
    {
        string key = $"{nameof(GameService)}.Cache.{GamePath}";

        if (memoryCache.TryGetValue(key, out object? value))
        {
            return new(true, Must.NotNull((value as string)!));
        }
        else
        {
            SettingEntry? entry = await appDbContext.Settings
                .SingleOrDefaultAsync(e => e.Key == GamePath)
                .ConfigureAwait(false);

            // Cannot find in setting
            if (entry == null)
            {
                // Create new setting
                entry = new(GamePath, null);

                // Try locate by registry
                IGameLocator locator = gameLocators.Single(l => l.Name == nameof(RegistryLauncherLocator));
                ValueResult<bool, string> result = await locator.LocateGamePathAsync().ConfigureAwait(false);

                if (!result.IsOk)
                {
                    // Try locate manually
                    locator = gameLocators.Single(l => l.Name == nameof(ManualGameLocator));
                    result = await locator.LocateGamePathAsync().ConfigureAwait(false);
                }

                if (result.IsOk)
                {
                    // Save result.
                    entry.Value = result.Value;
                    await appDbContext.Settings.AddAsync(entry).ConfigureAwait(false);
                    await appDbContext.SaveChangesAsync().ConfigureAwait(false);
                }
                else
                {
                    return new(false, null!);
                }
            }

            // Set cache and return.
            string path = memoryCache.Set(key, Must.NotNull(entry.Value!));
            return new(true, path);
        }
    }
}