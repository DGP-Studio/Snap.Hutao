// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Snap.Hutao.Context.Database;
using Snap.Hutao.Core.Threading;
using Snap.Hutao.Extension;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Game.Locator;

namespace Snap.Hutao.Service.Game;

/// <summary>
/// 游戏服务
/// </summary>
[Injection(InjectAs.Transient, typeof(IGameService))]
internal class GameService : IGameService
{
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
        string key = $"{nameof(GameService)}.Cache.{SettingEntry.GamePath}";

        if (memoryCache.TryGetValue(key, out object? value))
        {
            return new(true, Must.NotNull((value as string)!));
        }
        else
        {
            SettingEntry entry = appDbContext.Settings.SingleOrAdd(e => e.Key == SettingEntry.GamePath, () => new(SettingEntry.GamePath, null), out bool added);

            // Cannot find in setting
            if (added)
            {
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
                    appDbContext.Settings.Update(entry);
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