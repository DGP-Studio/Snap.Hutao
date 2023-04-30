// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Extensions.Caching.Memory;
using Snap.Hutao.Core.Database;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Core.IO.Ini;
using Snap.Hutao.Core.LifeCycle;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Entity.Database;
using Snap.Hutao.Service.Game.Locator;
using Snap.Hutao.Service.Game.Package;
using Snap.Hutao.View.Dialog;
using Snap.Hutao.Web.Hoyolab.SdkStatic.Hk4e.Launcher;
using Snap.Hutao.Web.Response;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using static Snap.Hutao.Service.Game.GameConstants;

namespace Snap.Hutao.Service.Game;

/// <summary>
/// 游戏服务
/// </summary>
[HighQuality]
[Injection(InjectAs.Singleton, typeof(IGameService))]
internal sealed class GameService : IGameService
{
    private const string GamePathKey = $"{nameof(GameService)}.Cache.{SettingEntry.GamePath}";

    private readonly ITaskContext taskContext;
    private readonly IMemoryCache memoryCache;
    private readonly PackageConverter packageConverter;
    private readonly LaunchOptions launchOptions;
    private readonly AppOptions appOptions;
    private readonly IServiceProvider serviceProvider;
    private volatile int runningGamesCounter;

    private ObservableCollection<GameAccount>? gameAccounts;

    /// <summary>
    /// 构造一个新的游戏服务
    /// </summary>
    /// <param name="serviceProvider">服务提供器</param>
    public GameService(IServiceProvider serviceProvider)
    {
        memoryCache = serviceProvider.GetRequiredService<IMemoryCache>();
        packageConverter = serviceProvider.GetRequiredService<PackageConverter>();
        launchOptions = serviceProvider.GetRequiredService<LaunchOptions>();
        appOptions = serviceProvider.GetRequiredService<AppOptions>();
        taskContext = serviceProvider.GetRequiredService<ITaskContext>();

        this.serviceProvider = serviceProvider;
    }

    /// <inheritdoc/>
    public ObservableCollection<GameAccount> GameAccountCollection
    {
        get
        {
            if (gameAccounts == null)
            {
                using (IServiceScope scope = serviceProvider.CreateScope())
                {
                    AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    gameAccounts = appDbContext.GameAccounts.ToObservableCollection();
                }
            }

            return gameAccounts;
        }
    }

    /// <inheritdoc/>
    public async ValueTask<ValueResult<bool, string>> GetGamePathAsync()
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            // Cannot find in setting
            if (string.IsNullOrEmpty(appOptions.GamePath))
            {
                IEnumerable<IGameLocator> gameLocators = scope.ServiceProvider.GetRequiredService<IEnumerable<IGameLocator>>();

                // Try locate by unity log
                ValueResult<bool, string> result = await gameLocators
                    .Pick(nameof(UnityLogGameLocator))
                    .LocateGamePathAsync()
                    .ConfigureAwait(false);

                if (!result.IsOk)
                {
                    // Try locate by registry
                    result = await gameLocators
                        .Pick(nameof(RegistryLauncherLocator))
                        .LocateGamePathAsync()
                        .ConfigureAwait(false);
                }

                if (result.IsOk)
                {
                    // Save result.
                    await taskContext.SwitchToMainThreadAsync();
                    appOptions.GamePath = result.Value;
                }
                else
                {
                    return new(false, SH.ServiceGamePathLocateFailed);
                }
            }

            if (!string.IsNullOrEmpty(appOptions.GamePath))
            {
                return new(true, appOptions.GamePath);
            }
            else
            {
                return new(false, null!);
            }
        }
    }

    /// <inheritdoc/>
    public MultiChannel GetMultiChannel()
    {
        string gamePath = appOptions.GamePath;
        string configPath = Path.Combine(Path.GetDirectoryName(gamePath) ?? string.Empty, ConfigFileName);

        if (!File.Exists(configPath))
        {
            return new(null, null, configPath);
        }

        using (FileStream stream = File.OpenRead(configPath))
        {
            IEnumerable<IniParameter> parameters = IniSerializer.Deserialize(stream).ToList().OfType<IniParameter>();
            string? channel = parameters.FirstOrDefault(p => p.Key == "channel")?.Value;
            string? subChannel = parameters.FirstOrDefault(p => p.Key == "sub_channel")?.Value;

            return new(channel, subChannel);
        }
    }

    /// <inheritdoc/>
    public bool SetMultiChannel(LaunchScheme scheme)
    {
        string gamePath = appOptions.GamePath;
        string configPath = Path.Combine(Path.GetDirectoryName(gamePath)!, ConfigFileName);

        List<IniElement> elements = null!;
        try
        {
            using (FileStream readStream = File.OpenRead(configPath))
            {
                elements = IniSerializer.Deserialize(readStream).ToList();
            }
        }
        catch (FileNotFoundException ex)
        {
            ThrowHelper.GameFileOperation(string.Format(SH.ServiceGameSetMultiChannelConfigFileNotFound, configPath), ex);
        }
        catch (DirectoryNotFoundException ex)
        {
            ThrowHelper.GameFileOperation(string.Format(SH.ServiceGameSetMultiChannelConfigFileNotFound, configPath), ex);
        }
        catch (UnauthorizedAccessException ex)
        {
            ThrowHelper.GameFileOperation(SH.ServiceGameSetMultiChannelUnauthorizedAccess, ex);
        }

        bool changed = false;

        foreach (IniElement element in elements)
        {
            if (element is IniParameter parameter)
            {
                if (parameter.Key == "channel")
                {
                    string channel = scheme.Channel.ToString("D");
                    if (parameter.Value != channel)
                    {
                        parameter.Value = channel;
                        changed = true;
                    }
                }

                if (parameter.Key == "sub_channel")
                {
                    string subChannel = scheme.SubChannel.ToString("D");
                    if (parameter.Value != subChannel)
                    {
                        parameter.Value = subChannel;
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

        return changed;
    }

    /// <inheritdoc/>
    public async Task<bool> EnsureGameResourceAsync(LaunchScheme launchScheme, IProgress<PackageReplaceStatus> progress)
    {
        string gamePath = appOptions.GamePath;
        string gameFolder = Path.GetDirectoryName(gamePath)!;
        string gameFileName = Path.GetFileName(gamePath);

        progress.Report(new(SH.ServiceGameEnsureGameResourceQueryResourceInformation));
        Response<GameResource> response;
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            response = await scope.ServiceProvider
                .GetRequiredService<ResourceClient>()
                .GetResourceAsync(launchScheme)
                .ConfigureAwait(false);
        }

        if (response.IsOk())
        {
            GameResource resource = response.Data;

            if (!LaunchSchemeMatchesExecutable(launchScheme, gameFileName))
            {
                bool replaced = await packageConverter
                    .EnsureGameResourceAsync(launchScheme, resource, gameFolder, progress)
                    .ConfigureAwait(false);

                if (replaced)
                {
                    // We need to change the gamePath if we switched.
                    string exeName = launchScheme.IsOversea ? GenshinImpactFileName : YuanShenFileName;

                    await taskContext.SwitchToMainThreadAsync();
                    appOptions.GamePath = Path.Combine(gameFolder, exeName);
                }
                else
                {
                    // We can't start the game
                    // when we failed to convert game
                    return false;
                }
            }

            if (!launchScheme.IsOversea)
            {
                await packageConverter.EnsureDeprecatedFilesAndSdkAsync(resource, gameFolder).ConfigureAwait(false);
            }

            return true;
        }

        return false;
    }

    /// <inheritdoc/>
    public bool IsGameRunning()
    {
        if (runningGamesCounter == 0)
        {
            return false;
        }

        if (launchOptions.MultipleInstances)
        {
            // If multiple instances is enabled, always treat as not running.
            return false;
        }

        return Process.GetProcessesByName(YuanShenProcessName).Any()
            || Process.GetProcessesByName(GenshinImpactProcessName).Any();
    }

    /// <inheritdoc/>
    public async ValueTask LaunchAsync()
    {
        if (IsGameRunning())
        {
            return;
        }

        string gamePath = appOptions.GamePath;
        if (string.IsNullOrWhiteSpace(gamePath))
        {
            // TODO: throw exception
            return;
        }

        Process game = ProcessInterop.PrepareGameProcess(launchOptions, gamePath);

        try
        {
            Interlocked.Increment(ref runningGamesCounter);

            game.Start();

            bool isAdvancedOptionsAllowed = Activation.GetElevated() && appOptions.IsAdvancedLaunchOptionsEnabled;
            if (isAdvancedOptionsAllowed && launchOptions.MultipleInstances)
            {
                ProcessInterop.DisableProtection(game, gamePath);
            }

            if (isAdvancedOptionsAllowed && launchOptions.UnlockFps)
            {
                await ProcessInterop.UnlockFpsAsync(game, launchOptions).ConfigureAwait(false);
            }
            else
            {
                await game.WaitForExitAsync().ConfigureAwait(false);
            }
        }
        finally
        {
            Interlocked.Decrement(ref runningGamesCounter);
        }
    }

    /// <inheritdoc/>
    public async ValueTask DetectGameAccountAsync()
    {
        Must.NotNull(gameAccounts!);

        string? registrySdk = RegistryInterop.Get();
        if (!string.IsNullOrEmpty(registrySdk))
        {
            GameAccount? account = null;
            try
            {
                account = gameAccounts.SingleOrDefault(a => a.MihoyoSDK == registrySdk);
            }
            catch (InvalidOperationException ex)
            {
                ThrowHelper.UserdataCorrupted(SH.ServiceGameDetectGameAccountMultiMatched, ex);
            }

            if (account == null)
            {
                // ContentDialog must be created by main thread.
                await taskContext.SwitchToMainThreadAsync();
                LaunchGameAccountNameDialog dialog = serviceProvider.CreateInstance<LaunchGameAccountNameDialog>();
                (bool isOk, string name) = await dialog.GetInputNameAsync().ConfigureAwait(false);

                if (isOk)
                {
                    account = GameAccount.Create(name, registrySdk);

                    // sync database
                    await taskContext.SwitchToBackgroundAsync();
                    using (IServiceScope scope = serviceProvider.CreateScope())
                    {
                        await scope.ServiceProvider
                            .GetRequiredService<AppDbContext>()
                            .GameAccounts
                            .AddAndSaveAsync(account)
                            .ConfigureAwait(false);
                    }

                    // sync cache
                    await taskContext.SwitchToMainThreadAsync();
                    gameAccounts.Add(account);
                }
            }
        }
    }

    /// <inheritdoc/>
    public GameAccount? DetectCurrentGameAccount()
    {
        Must.NotNull(gameAccounts!);

        string? registrySdk = RegistryInterop.Get();

        if (!string.IsNullOrEmpty(registrySdk))
        {
            try
            {
                return gameAccounts.SingleOrDefault(a => a.MihoyoSDK == registrySdk);
            }
            catch (InvalidOperationException ex)
            {
                ThrowHelper.UserdataCorrupted(SH.ServiceGameDetectGameAccountMultiMatched, ex);
            }
        }

        return null;
    }

    /// <inheritdoc/>
    public bool SetGameAccount(GameAccount account)
    {
        return RegistryInterop.Set(account);
    }

    /// <inheritdoc/>
    public void AttachGameAccountToUid(GameAccount gameAccount, string uid)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            gameAccount.UpdateAttachUid(uid);
            scope.ServiceProvider.GetRequiredService<AppDbContext>().GameAccounts.UpdateAndSave(gameAccount);
        }
    }

    /// <inheritdoc/>
    public async ValueTask ModifyGameAccountAsync(GameAccount gameAccount)
    {
        await taskContext.SwitchToMainThreadAsync();
        LaunchGameAccountNameDialog dialog = serviceProvider.CreateInstance<LaunchGameAccountNameDialog>();
        (bool isOk, string name) = await dialog.GetInputNameAsync().ConfigureAwait(true);

        if (isOk)
        {
            gameAccount.UpdateName(name);

            // sync database
            await taskContext.SwitchToBackgroundAsync();
            using (IServiceScope scope = serviceProvider.CreateScope())
            {
                AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                await appDbContext.GameAccounts.UpdateAndSaveAsync(gameAccount).ConfigureAwait(false);
            }
        }
    }

    /// <inheritdoc/>
    public async ValueTask RemoveGameAccountAsync(GameAccount gameAccount)
    {
        await taskContext.SwitchToMainThreadAsync();
        gameAccounts!.Remove(gameAccount);

        await taskContext.SwitchToBackgroundAsync();
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            await scope.ServiceProvider.GetRequiredService<AppDbContext>().GameAccounts.RemoveAndSaveAsync(gameAccount).ConfigureAwait(false);
        }
    }

    private static bool LaunchSchemeMatchesExecutable(LaunchScheme launchScheme, string gameFileName)
    {
        return (launchScheme.IsOversea && gameFileName == GenshinImpactFileName)
            || (!launchScheme.IsOversea && gameFileName == YuanShenFileName);
    }
}