// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Snap.Hutao.Context.Database;
using Snap.Hutao.Core;
using Snap.Hutao.Core.Database;
using Snap.Hutao.Core.Threading;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Game.Locator;
using Snap.Hutao.Service.Game.Unlocker;
using System.Diagnostics;
using System.IO;

namespace Snap.Hutao.Service.Game;

/// <summary>
/// 游戏服务
/// </summary>
[Injection(InjectAs.Singleton, typeof(IGameService))]
internal class GameService : IGameService
{
    private const string GamePathKey = $"{nameof(GameService)}.Cache.{SettingEntry.GamePath}";

    private readonly IServiceScopeFactory scopeFactory;
    private readonly IMemoryCache memoryCache;
    private readonly SemaphoreSlim gameSemaphore = new(1);

    /// <summary>
    /// 构造一个新的游戏服务
    /// </summary>
    /// <param name="scopeFactory">范围工厂</param>
    /// <param name="memoryCache">内存缓存</param>
    /// <param name="gameLocators">游戏定位器集合</param>
    public GameService(IServiceScopeFactory scopeFactory, IMemoryCache memoryCache)
    {
        this.scopeFactory = scopeFactory;
        this.memoryCache = memoryCache;
    }

    /// <inheritdoc/>
    public async ValueTask<ValueResult<bool, string>> GetGamePathAsync()
    {
        if (memoryCache.TryGetValue(GamePathKey, out object? value))
        {
            return new(true, Must.NotNull((value as string)!));
        }
        else
        {
            using (IServiceScope scope = scopeFactory.CreateScope())
            {
                AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                SettingEntry entry = appDbContext.Settings.SingleOrAdd(e => e.Key == SettingEntry.GamePath, () => new(SettingEntry.GamePath, null), out bool added);

                // Cannot find in setting
                if (added)
                {
                    IEnumerable<IGameLocator> gameLocators = scope.ServiceProvider.GetRequiredService<IEnumerable<IGameLocator>>();

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
                        appDbContext.Settings.UpdateAndSave(entry);
                    }
                    else
                    {
                        return new(false, null!);
                    }
                }

                // Set cache and return.
                string path = memoryCache.Set(GamePathKey, Must.NotNull(entry.Value!));
                return new(true, path);
            }
        }
    }

    /// <inheritdoc/>
    public string GetGamePathSkipLocator()
    {
        if (memoryCache.TryGetValue(GamePathKey, out object? value))
        {
            return (value as string)!;
        }
        else
        {
            using (IServiceScope scope = scopeFactory.CreateScope())
            {
                AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                SettingEntry entry = appDbContext.Settings.SingleOrAdd(e => e.Key == SettingEntry.GamePath, () => new(SettingEntry.GamePath, null), out bool added);

                entry.Value ??= string.Empty;
                appDbContext.Settings.UpdateAndSave(entry);

                // Set cache and return.
                return memoryCache.Set(GamePathKey, entry.Value);
            }
        }
    }

    /// <inheritdoc/>
    public void OverwriteGamePath(string path)
    {
        // sync cache
        memoryCache.Set(GamePathKey, path);

        using (IServiceScope scope = scopeFactory.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            SettingEntry entry = appDbContext.Settings.SingleOrAdd(e => e.Key == SettingEntry.GamePath, () => new(SettingEntry.GamePath, null), out _);
            entry.Value = path;
            appDbContext.Settings.UpdateAndSave(entry);
        }
    }

    /// <inheritdoc/>
    public async ValueTask LaunchAsync(LaunchConfiguration configuration)
    {
        (bool isOk, string gamePath) = await GetGamePathAsync().ConfigureAwait(false);

        if (isOk)
        {
            if (gameSemaphore.CurrentCount == 0)
            {
                return;
            }

            string commandLine = new CommandLineBuilder()
                .Append("-window-mode", configuration.WindowMode)
                .Append("-screen-fullscreen", configuration.IsFullScreen ? 1 : 0)
                .Append("-screen-width", configuration.ScreenWidth)
                .Append("-screen-height", configuration.ScreenHeight)
                .Append("-monitor", configuration.Monitor)
                .Build();

            Process game = new()
            {
                StartInfo = new()
                {
                    Arguments = commandLine,
                    FileName = gamePath,
                    UseShellExecute = true,
                    Verb = "runas",
                    WorkingDirectory = Path.GetDirectoryName(gamePath),
                },
            };

            using (await gameSemaphore.EnterAsync().ConfigureAwait(false))
            {
                if (configuration.UnlockFPS)
                {
                    IGameFpsUnlocker unlocker = new GameFpsUnlocker(game, configuration.TargetFPS);

                    await unlocker.UnlockAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(10000), TimeSpan.FromMilliseconds(2000)).ConfigureAwait(false);
                }
                else
                {
                    if (game.Start())
                    {
                        await game.WaitForExitAsync().ConfigureAwait(false);
                    }
                }
            }
        }
    }
}