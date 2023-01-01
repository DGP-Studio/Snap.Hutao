// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Snap.Hutao.Core;
using Snap.Hutao.Core.Database;
using Snap.Hutao.Core.IO.Ini;
using Snap.Hutao.Model.Binding.LaunchGame;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Entity.Database;
using Snap.Hutao.Service.Game.Locator;
using Snap.Hutao.Service.Game.Unlocker;
using Snap.Hutao.View.Dialog;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;

namespace Snap.Hutao.Service.Game;

/// <summary>
/// 游戏服务
/// </summary>
[Injection(InjectAs.Singleton, typeof(IGameService))]
internal class GameService : IGameService, IDisposable
{
    private const string GamePathKey = $"{nameof(GameService)}.Cache.{SettingEntry.GamePath}";
    private const string ConfigFile = "config.ini";

    private readonly IServiceScopeFactory scopeFactory;
    private readonly IMemoryCache memoryCache;
    private readonly SemaphoreSlim gameSemaphore = new(1);

    private ObservableCollection<GameAccount>? gameAccounts;

    /// <summary>
    /// 构造一个新的游戏服务
    /// </summary>
    /// <param name="scopeFactory">范围工厂</param>
    /// <param name="memoryCache">内存缓存</param>
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

                SettingEntry entry = await appDbContext.Settings.SingleOrAddAsync(SettingEntry.GamePath, string.Empty).ConfigureAwait(false);

                // Cannot find in setting
                if (string.IsNullOrEmpty(entry.Value))
                {
                    IEnumerable<IGameLocator> gameLocators = scope.ServiceProvider.GetRequiredService<IEnumerable<IGameLocator>>();

                    // Try locate by registry
                    IGameLocator locator = gameLocators.Single(l => l.Name == nameof(UnityLogGameLocator));
                    ValueResult<bool, string> result = await locator.LocateGamePathAsync().ConfigureAwait(false);

                    if (!result.IsOk)
                    {
                        // Try locate manually
                        locator = gameLocators.Single(l => l.Name == nameof(RegistryLauncherLocator));
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
                        return new(false, "请启动游戏后再次尝试");
                    }
                }

                if (entry.Value == null)
                {
                    return new(false, null!);
                }

                // Set cache and return.
                string path = memoryCache.Set(GamePathKey, entry.Value);
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
                SettingEntry entry = appDbContext.Settings.SingleOrAdd(SettingEntry.GamePath, string.Empty);

                // Set cache and return.
                return memoryCache.Set(GamePathKey, entry.Value!);
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

            SettingEntry entry = appDbContext.Settings.SingleOrAdd(SettingEntry.GamePath, string.Empty);
            entry.Value = path;
            appDbContext.Settings.UpdateAndSave(entry);
        }
    }

    /// <inheritdoc/>
    public MultiChannel GetMultiChannel()
    {
        string gamePath = GetGamePathSkipLocator();
        string configPath = Path.Combine(Path.GetDirectoryName(gamePath) ?? string.Empty, ConfigFile);

        using (FileStream stream = File.OpenRead(configPath))
        {
            List<IniElement> elements = IniSerializer.Deserialize(stream).ToList();
            string? channel = elements.OfType<IniParameter>().FirstOrDefault(p => p.Key == "channel")?.Value;
            string? subChannel = elements.OfType<IniParameter>().FirstOrDefault(p => p.Key == "sub_channel")?.Value;

            return new(channel, subChannel);
        }
    }

    /// <inheritdoc/>
    public void SetMultiChannel(LaunchScheme scheme)
    {
        string gamePath = GetGamePathSkipLocator();
        string configPath = Path.Combine(Path.GetDirectoryName(gamePath)!, ConfigFile);

        List<IniElement> elements;
        using (FileStream readStream = File.OpenRead(configPath))
        {
            elements = IniSerializer.Deserialize(readStream).ToList();
        }

        bool changed = false;

        foreach (IniElement element in elements)
        {
            if (element is IniParameter parameter)
            {
                if (parameter.Key == "channel")
                {
                    if (parameter.Value != scheme.Channel)
                    {
                        parameter.Value = scheme.Channel;
                        changed = true;
                    }
                }

                if (parameter.Key == "sub_channel")
                {
                    if (parameter.Value != scheme.SubChannel)
                    {
                        parameter.Value = scheme.SubChannel;
                        changed = true;
                    }
                }
            }
        }

        if (changed)
        {
            using (FileStream writeStream = File.Create(configPath))
            {
                IniSerializer.Serialize(writeStream, elements);
            }
        }
    }

    /// <inheritdoc/>
    public bool IsGameRunning()
    {
        if (gameSemaphore.CurrentCount == 0)
        {
            return true;
        }

        return Process.GetProcessesByName("YuanShen.exe").Any();
    }

    /// <inheritdoc/>
    public ObservableCollection<GameAccount> GetGameAccountCollection()
    {
        if (gameAccounts == null)
        {
            using (IServiceScope scope = scopeFactory.CreateScope())
            {
                AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                gameAccounts = new(appDbContext.GameAccounts.AsNoTracking().ToList());
            }
        }

        return gameAccounts;
    }

    /// <inheritdoc/>
    public async ValueTask LaunchAsync(LaunchConfiguration configuration)
    {
        if (IsGameRunning())
        {
            return;
        }

        string gamePath = GetGamePathSkipLocator();

        if (string.IsNullOrWhiteSpace(gamePath))
        {
            return;
        }

        // https://docs.unity.cn/cn/current/Manual/PlayerCommandLineArguments.html
        string commandLine = new CommandLineBuilder()
            .AppendIf("-popupwindow", configuration.IsBorderless)
            .Append("-screen-fullscreen", configuration.IsFullScreen ? 1 : 0)
            .AppendIf("-window-mode", configuration.IsExclusive, "exclusive")
            .Append("-screen-width", configuration.ScreenWidth)
            .Append("-screen-height", configuration.ScreenHeight)
            .ToString();

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
            try
            {
                if (configuration.UnlockFPS)
                {
                    IGameFpsUnlocker unlocker = new GameFpsUnlocker(game, configuration.TargetFPS);

                    TimeSpan findModuleDelay = TimeSpan.FromMilliseconds(100);
                    TimeSpan findModuleLimit = TimeSpan.FromMilliseconds(10000);
                    TimeSpan adjustFpsDelay = TimeSpan.FromMilliseconds(2000);
                    if (game.Start())
                    {
                        await unlocker.UnlockAsync(findModuleDelay, findModuleLimit, adjustFpsDelay).ConfigureAwait(false);
                    }
                }
                else
                {
                    if (game.Start())
                    {
                        await game.WaitForExitAsync().ConfigureAwait(false);
                    }
                }
            }
            catch (Win32Exception)
            {
                // 通常是用户取消了UAC
            }
        }
    }

    /// <inheritdoc/>
    public async ValueTask DetectGameAccountAsync()
    {
        Must.NotNull(gameAccounts!);

        string? registrySdk = GameAccountRegistryInterop.Get();
        if (!string.IsNullOrEmpty(registrySdk))
        {
            GameAccount? account = gameAccounts.SingleOrDefault(a => a.MihoyoSDK == registrySdk);

            if (account == null)
            {
                MainWindow mainWindow = Ioc.Default.GetRequiredService<MainWindow>();
                (bool isOk, string name) = await new GameAccountNameDialog(mainWindow).GetInputNameAsync().ConfigureAwait(false);

                if (isOk)
                {
                    account = GameAccount.Create(name, registrySdk);

                    // sync database
                    await ThreadHelper.SwitchToBackgroundAsync();
                    using (IServiceScope scope = scopeFactory.CreateScope())
                    {
                        await scope.ServiceProvider
                            .GetRequiredService<AppDbContext>()
                            .GameAccounts
                            .AddAndSaveAsync(account)
                            .ConfigureAwait(false);
                    }

                    // sync cache
                    await ThreadHelper.SwitchToMainThreadAsync();
                    gameAccounts.Add(account);
                }
            }
        }
    }

    /// <inheritdoc/>
    public bool SetGameAccount(GameAccount account)
    {
        return GameAccountRegistryInterop.Set(account);
    }

    /// <inheritdoc/>
    public void AttachGameAccountToUid(GameAccount gameAccount, string uid)
    {
        using (IServiceScope scope = scopeFactory.CreateScope())
        {
            gameAccount.UpdateAttachUid(uid);
            scope.ServiceProvider.GetRequiredService<AppDbContext>().GameAccounts.UpdateAndSave(gameAccount);
        }
    }

    /// <inheritdoc/>
    public async ValueTask ModifyGameAccountAsync(GameAccount gameAccount)
    {
        MainWindow mainWindow = Ioc.Default.GetRequiredService<MainWindow>();
        (bool isOk, string name) = await new GameAccountNameDialog(mainWindow).GetInputNameAsync().ConfigureAwait(true);

        if (isOk)
        {
            gameAccount.UpdateName(name);

            // sync database
            await ThreadHelper.SwitchToBackgroundAsync();
            using (IServiceScope scope = scopeFactory.CreateScope())
            {
                AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                await appDbContext.GameAccounts.UpdateAndSaveAsync(gameAccount).ConfigureAwait(false);
            }
        }
    }

    /// <inheritdoc/>
    public async ValueTask RemoveGameAccountAsync(GameAccount gameAccount)
    {
        await ThreadHelper.SwitchToMainThreadAsync();
        gameAccounts!.Remove(gameAccount);

        await ThreadHelper.SwitchToBackgroundAsync();
        using (IServiceScope scope = scopeFactory.CreateScope())
        {
            await scope.ServiceProvider.GetRequiredService<AppDbContext>().GameAccounts.RemoveAndSaveAsync(gameAccount).ConfigureAwait(false);
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        gameSemaphore?.Dispose();
    }
}