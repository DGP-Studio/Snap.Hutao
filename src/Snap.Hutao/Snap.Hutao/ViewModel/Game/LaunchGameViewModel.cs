﻿// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Extensions.Caching.Memory;
using Snap.Hutao.Control.Extension;
using Snap.Hutao.Core;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Core.LifeCycle;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service;
using Snap.Hutao.Service.Game;
using Snap.Hutao.Service.Game.Locator;
using Snap.Hutao.Service.Navigation;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.Service.User;
using Snap.Hutao.View.Dialog;
using Snap.Hutao.Web.Hoyolab.SdkStatic.Hk4e.Launcher;
using Snap.Hutao.Web.Hoyolab.Takumi.Binding;
using System.Collections.ObjectModel;
using System.IO;

namespace Snap.Hutao.ViewModel.Game;

/// <summary>
/// 启动游戏视图模型
/// </summary>
[HighQuality]
[ConstructorGenerated]
[Injection(InjectAs.Scoped)]
internal sealed partial class LaunchGameViewModel : Abstraction.ViewModel
{
    /// <summary>
    /// 启动游戏目标 Uid
    /// </summary>
    public const string DesiredUid = nameof(DesiredUid);

    private readonly IServiceProvider serviceProvider;
    private readonly LaunchOptions launchOptions;
    private readonly HutaoOptions hutaoOptions;
    private readonly ITaskContext taskContext;
    private readonly IGameService gameService;
    private readonly IMemoryCache memoryCache;
    private readonly AppOptions appOptions;

    private LaunchScheme? selectedScheme;
    private ObservableCollection<GameAccount>? gameAccounts;
    private GameAccount? selectedGameAccount;
    private GameResource? gameResource;

    /// <summary>
    /// 已知的服务器方案
    /// </summary>
    public List<LaunchScheme> KnownSchemes { get => LaunchScheme.GetKnownSchemes(); }

    /// <summary>
    /// 当前选择的服务器方案
    /// </summary>
    public LaunchScheme? SelectedScheme
    {
        get => selectedScheme; set
        {
            if (SetProperty(ref selectedScheme, value))
            {
                if (value != null)
                {
                    UpdateGameResourceAsync(value).SafeForget();
                }
            }
        }
    }

    /// <summary>
    /// 游戏账号集合
    /// </summary>
    public ObservableCollection<GameAccount>? GameAccounts { get => gameAccounts; set => SetProperty(ref gameAccounts, value); }

    /// <summary>
    /// 选中的账号
    /// </summary>
    public GameAccount? SelectedGameAccount { get => selectedGameAccount; set => SetProperty(ref selectedGameAccount, value); }

    /// <summary>
    /// 启动选项
    /// </summary>
    public LaunchOptions Options { get => launchOptions; }

    /// <summary>
    /// 胡桃选项
    /// </summary>
    public HutaoOptions HutaoOptions { get => hutaoOptions; }

    /// <summary>
    /// 应用选项
    /// </summary>
    public AppOptions AppOptions { get => appOptions; }

    /// <summary>
    /// 游戏资源
    /// </summary>
    public GameResource? GameResource { get => gameResource; set => SetProperty(ref gameResource, value); }

    /// <summary>
    /// 展示风险功能的可见性
    /// </summary>
    public bool AdvencedFeatureVisibility
    {
        get => hutaoOptions.IsElevated && AppOptions.IsAdvancedLaunchOptionsEnabled && !gameService.IsSwitchToStarRailTools;
    }

    /// <summary>
    /// 选择 DLL 路径的可见性
    /// </summary>
    public bool ChooseDllPathVisibility
    {
        get => (AppOptions.IsAdvancedLaunchOptionsEnabled && launchOptions.DllInjector) || AdvencedFeatureVisibility;
    }

    /// <inheritdoc/>
    protected override async Task OpenUIAsync()
    {
        IInfoBarService infoBarService = serviceProvider.GetRequiredService<IInfoBarService>();

        if (File.Exists(AppOptions.GamePath))
        {
            try
            {
                using (await EnterCriticalExecutionAsync().ConfigureAwait(false))
                {
                    ChannelOptions options = gameService.GetChannelOptions();
                    if (string.IsNullOrEmpty(options.ConfigFilePath))
                    {
                        try
                        {
                            SelectedScheme = KnownSchemes
                                .Where(scheme => scheme.IsOversea == options.IsOversea)
                                .Single(scheme => scheme.MultiChannelEqual(options));
                        }
                        catch (InvalidOperationException)
                        {
                            // 后台收集
                            throw new NotSupportedException($"不支持的 MultiChannel: {options}");
                        }
                    }
                    else
                    {
                        infoBarService.Warning(string.Format(SH.ViewModelLaunchGameMultiChannelReadFail, options.ConfigFilePath));
                    }

                    ObservableCollection<GameAccount> accounts = gameService.GameAccountCollection;

                    await taskContext.SwitchToMainThreadAsync();
                    GameAccounts = accounts;

                    // Sync uid
                    if (memoryCache.TryRemove(DesiredUid, out object? value) && value is string uid)
                    {
                        SelectedGameAccount = GameAccounts.FirstOrDefault(g => g.AttachUid == uid);
                    }

                    // Try set to the current account.
                    SelectedGameAccount ??= gameService.DetectCurrentGameAccount();
                }
            }
            catch (OperationCanceledException)
            {
            }
        }
        else
        {
            infoBarService.Warning(SH.ViewModelLaunchGamePathInvalid);
            await taskContext.SwitchToMainThreadAsync();
            await serviceProvider.GetRequiredService<INavigationService>()
                .NavigateAsync<View.Page.SettingPage>(INavigationAwaiter.Default, true)
                .ConfigureAwait(false);
        }
    }

    private async Task UpdateGameResourceAsync(LaunchScheme scheme)
    {
        await taskContext.SwitchToBackgroundAsync();
        Web.Response.Response<GameResource> response = await serviceProvider
            .GetRequiredService<ResourceClient>()
            .GetResourceAsync(scheme)
            .ConfigureAwait(false);

        if (response.IsOk())
        {
            await taskContext.SwitchToMainThreadAsync();
            GameResource = response.Data;
        }
    }

    [Command("LaunchCommand", AllowConcurrentExecutions = true)]
    private async Task LaunchAsync()
    {
        IInfoBarService infoBarService = serviceProvider.GetRequiredService<IInfoBarService>();

        if (SelectedScheme != null)
        {
            try
            {
                if (gameService.SetChannelOptions(SelectedScheme))
                {
                    // Channel changed, we need to change local file.
                    await taskContext.SwitchToMainThreadAsync();
                    LaunchGamePackageConvertDialog dialog = serviceProvider.CreateInstance<LaunchGamePackageConvertDialog>();
                    Progress<Service.Game.Package.PackageReplaceStatus> progress = new(state => dialog.State = state.Clone());
                    using (await dialog.BlockAsync(taskContext).ConfigureAwait(false))
                    {
                        if (!await gameService.EnsureGameResourceAsync(SelectedScheme, progress).ConfigureAwait(false))
                        {
                            infoBarService.Warning(SH.ViewModelLaunchGameEnsureGameResourceFail);
                            return;
                        }
                    }
                }

                if (SelectedGameAccount != null)
                {
                    if (!gameService.SetGameAccount(SelectedGameAccount))
                    {
                        infoBarService.Warning(SH.ViewModelLaunchGameSwitchGameAccountFail);
                        return;
                    }
                }

                await gameService.LaunchAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                infoBarService.Error(ex);
            }
        }
        else
        {
            infoBarService.Error(SH.ViewModelLaunchGameSchemeNotSelected);
        }
    }

    [Command("DetectGameAccountCommand")]
    private async Task DetectGameAccountAsync()
    {
        try
        {
            await gameService.DetectGameAccountAsync().ConfigureAwait(false);
        }
        catch (UserdataCorruptedException ex)
        {
            serviceProvider.GetRequiredService<IInfoBarService>().Error(ex);
        }
    }

    [Command("AttachGameAccountCommand")]
    private void AttachGameAccountToCurrentUserGameRole(GameAccount? gameAccount)
    {
        if (gameAccount != null)
        {
            IUserService userService = serviceProvider.GetRequiredService<IUserService>();
            IInfoBarService infoBarService = serviceProvider.GetRequiredService<IInfoBarService>();

            if (userService.Current?.SelectedUserGameRole is UserGameRole role)
            {
                gameService.AttachGameAccountToUid(gameAccount, role.GameUid);
            }
            else
            {
                infoBarService.Warning(SH.MustSelectUserAndUid);
            }
        }
    }

    [Command("ModifyGameAccountCommand")]
    private async Task ModifyGameAccountAsync(GameAccount? gameAccount)
    {
        if (gameAccount != null)
        {
            await gameService.ModifyGameAccountAsync(gameAccount).ConfigureAwait(false);
        }
    }

    [Command("RemoveGameAccountCommand")]
    private async Task RemoveGameAccountAsync(GameAccount? gameAccount)
    {
        if (gameAccount != null)
        {
            await gameService.RemoveGameAccountAsync(gameAccount).ConfigureAwait(false);
        }
    }

    [Command("OpenScreenshotFolderCommand")]
    private async Task OpenScreenshotFolderAsync()
    {
        string game = serviceProvider.GetRequiredService<AppOptions>().GamePath;
        string screenshot = Path.Combine(Path.GetDirectoryName(game)!, "ScreenShot");
        if (Directory.Exists(screenshot))
        {
            await Windows.System.Launcher.LaunchFolderPathAsync(screenshot);
        }
    }

    [Command("SetDllPathCommand")]
    private async Task SetDllPathAsync()
    {
        IGameLocator locator = serviceProvider.GetRequiredService<IEnumerable<IGameLocator>>().Pick(nameof(ManualGameLocator));

        (bool isOk, string path) = await locator.LocateGamePathAsync(new(false, true)).ConfigureAwait(false);
        if (isOk)
        {
            await taskContext.SwitchToMainThreadAsync();
            Options.DllPath = path;
        }
    }

    [Command("RestartAsElevatedCommand")]
    private async Task RestartAsElevatedAsync()
    {
        try
        {
            await hutaoOptions.RestartAsElevatedAsync();
        }
        catch (Exception ex)
        {
            serviceProvider.GetRequiredService<IInfoBarService>().Error(ex);
        }
    }
}