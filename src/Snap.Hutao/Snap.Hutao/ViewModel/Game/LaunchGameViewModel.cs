// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Extensions.Caching.Memory;
using Snap.Hutao.Control.Extension;
using Snap.Hutao.Core;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Factory.Abstraction;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service;
using Snap.Hutao.Service.Game;
using Snap.Hutao.Service.Game.Package;
using Snap.Hutao.Service.Navigation;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.Service.User;
using Snap.Hutao.View.Dialog;
using Snap.Hutao.Web.Hoyolab.SdkStatic.Hk4e.Launcher;
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

    private readonly IContentDialogFactory contentDialogFactory;
    private readonly LaunchStatusOptions launchStatusOptions;
    private readonly INavigationService navigationService;
    private readonly IInfoBarService infoBarService;
    private readonly ResourceClient resourceClient;
    private readonly LaunchOptions launchOptions;
    private readonly RuntimeOptions hutaoOptions;
    private readonly IUserService userService;
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
    [SuppressMessage("", "CA1822")]
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
                if (value is not null)
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

    public LaunchStatusOptions LaunchStatusOptions { get => launchStatusOptions; }

    /// <summary>
    /// 胡桃选项
    /// </summary>
    public RuntimeOptions HutaoOptions { get => hutaoOptions; }

    /// <summary>
    /// 应用选项
    /// </summary>
    public AppOptions AppOptions { get => appOptions; }

    /// <summary>
    /// 游戏资源
    /// </summary>
    public GameResource? GameResource { get => gameResource; set => SetProperty(ref gameResource, value); }

    protected override async ValueTask<bool> InitializeUIAsync()
    {
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
                            if (!IgnoredInvalidChannelOptions.Contains(options))
                            {
                                // 后台收集
                                throw new NotSupportedException($"不支持的 MultiChannel: {options}");
                            }
                        }
                    }
                    else
                    {
                        infoBarService.Warning(SH.ViewModelLaunchGameMultiChannelReadFail.Format(options.ConfigFilePath));
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
            catch (UserdataCorruptedException ex)
            {
                infoBarService.Error(ex);
            }
            catch (OperationCanceledException)
            {
            }
        }
        else
        {
            infoBarService.Warning(SH.ViewModelLaunchGamePathInvalid);
            await taskContext.SwitchToMainThreadAsync();
            await navigationService
                .NavigateAsync<View.Page.SettingPage>(INavigationAwaiter.Default, true)
                .ConfigureAwait(false);
        }

        return true;
    }

    private async ValueTask UpdateGameResourceAsync(LaunchScheme scheme)
    {
        await taskContext.SwitchToBackgroundAsync();
        Web.Response.Response<GameResource> response = await resourceClient
            .GetResourceAsync(scheme)
            .ConfigureAwait(false);

        if (response.IsOk())
        {
            await taskContext.SwitchToMainThreadAsync();
            GameResource = response.Data;
        }
    }

    [Command("LaunchCommand")]
    private async Task LaunchAsync()
    {
        if (SelectedScheme is not null)
        {
            try
            {
                gameService.SetChannelOptions(SelectedScheme);

                // Whether or not the channel options changed, we always ensure game resouces
                LaunchGamePackageConvertDialog dialog = await contentDialogFactory.CreateInstanceAsync<LaunchGamePackageConvertDialog>().ConfigureAwait(false);
                IProgress<PackageReplaceStatus> convertProgress = taskContext.CreateProgressForMainThread<PackageReplaceStatus>(state => dialog.State = state);
                using (await dialog.BlockAsync(taskContext).ConfigureAwait(false))
                {
                    if (!await gameService.EnsureGameResourceAsync(SelectedScheme, convertProgress).ConfigureAwait(false))
                    {
                        infoBarService.Warning(SH.ViewModelLaunchGameEnsureGameResourceFail);
                        return;
                    }
                }

                if (SelectedGameAccount is not null)
                {
                    if (!gameService.SetGameAccount(SelectedGameAccount))
                    {
                        infoBarService.Warning(SH.ViewModelLaunchGameSwitchGameAccountFail);
                        return;
                    }
                }

                IProgress<LaunchStatus> launchProgress = taskContext.CreateProgressForMainThread<LaunchStatus>(status => launchStatusOptions.LaunchStatus = status);
                await gameService.LaunchAsync(launchProgress).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ExceptionFormat.Format(ex));
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
            GameAccount? account = await gameService.DetectGameAccountAsync().ConfigureAwait(false);

            // If user canceled the operation, the return is null,
            // and thus we should not set SelectedAccount
            if (account is not null)
            {
                await taskContext.SwitchToMainThreadAsync();
                SelectedGameAccount = account;
            }
        }
        catch (UserdataCorruptedException ex)
        {
            infoBarService.Error(ex);
        }
    }

    [Command("AttachGameAccountCommand")]
    private void AttachGameAccountToCurrentUserGameRole(GameAccount? gameAccount)
    {
        if (gameAccount is not null)
        {
            if (userService.Current?.SelectedUserGameRole is { } role)
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
        if (gameAccount is not null)
        {
            await gameService.ModifyGameAccountAsync(gameAccount).ConfigureAwait(false);
        }
    }

    [Command("RemoveGameAccountCommand")]
    private async Task RemoveGameAccountAsync(GameAccount? gameAccount)
    {
        if (gameAccount is not null)
        {
            await gameService.RemoveGameAccountAsync(gameAccount).ConfigureAwait(false);
        }
    }

    [Command("OpenScreenshotFolderCommand")]
    private async Task OpenScreenshotFolderAsync()
    {
        string game = appOptions.GamePath;
        string? directory = Path.GetDirectoryName(game);
        ArgumentException.ThrowIfNullOrEmpty(directory);
        string screenshot = Path.Combine(directory, "ScreenShot");
        if (Directory.Exists(screenshot))
        {
            await Windows.System.Launcher.LaunchFolderPathAsync(screenshot);
        }
    }
}