// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI.Collections;
using Microsoft.Extensions.Caching.Memory;
using Snap.Hutao.Control.Extension;
using Snap.Hutao.Core;
using Snap.Hutao.Core.Diagnostics.CodeAnalysis;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.Factory.Progress;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service;
using Snap.Hutao.Service.Game;
using Snap.Hutao.Service.Game.Locator;
using Snap.Hutao.Service.Game.Package;
using Snap.Hutao.Service.Game.PathAbstraction;
using Snap.Hutao.Service.Game.Scheme;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.Service.User;
using Snap.Hutao.View.Dialog;
using Snap.Hutao.Web.Hoyolab.SdkStatic.Hk4e.Launcher;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.IO;
using Windows.Win32.Foundation;

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
    private readonly IGameLocatorFactory gameLocatorFactory;
    private readonly ILogger<LaunchGameViewModel> logger;
    private readonly IProgressFactory progressFactory;
    private readonly IInfoBarService infoBarService;
    private readonly ResourceClient resourceClient;
    private readonly RuntimeOptions runtimeOptions;
    private readonly LaunchOptions launchOptions;
    private readonly IUserService userService;
    private readonly ITaskContext taskContext;
    private readonly IGameServiceFacade gameService;
    private readonly IMemoryCache memoryCache;
    private readonly AppOptions appOptions;

    private LaunchScheme? selectedScheme;
    private AdvancedCollectionView? gameAccountsView;
    private GameAccount? selectedGameAccount;
    private GameResource? gameResource;
    private bool gamePathSelectedAndValid;
    private ImmutableList<GamePathEntry> gamePathEntries;
    private GamePathEntry? selectedGamePathEntry;
    private GameAccountFilter? gameAccountFilter;

    public LaunchOptions LaunchOptions { get => launchOptions; }

    public LaunchStatusOptions LaunchStatusOptions { get => launchStatusOptions; }

    public RuntimeOptions RuntimeOptions { get => runtimeOptions; }

    public AppOptions AppOptions { get => appOptions; }

    public List<LaunchScheme> KnownSchemes { get; } = KnownLaunchSchemes.Get();

    [AlsoAsyncSets(nameof(GameResource), nameof(GameAccountsView))]
    public LaunchScheme? SelectedScheme
    {
        get => selectedScheme;
        set => SetSelectedSchemeAsync(value).SafeForget();
    }

    public AdvancedCollectionView? GameAccountsView { get => gameAccountsView; set => SetProperty(ref gameAccountsView, value); }

    public GameAccount? SelectedGameAccount { get => selectedGameAccount; set => SetProperty(ref selectedGameAccount, value); }

    public GameResource? GameResource { get => gameResource; set => SetProperty(ref gameResource, value); }

    [AlsoAsyncSets(nameof(SelectedScheme), nameof(GameAccountsView))]
    public bool GamePathSelectedAndValid
    {
        get => gamePathSelectedAndValid;
        set
        {
            if (SetProperty(ref gamePathSelectedAndValid, value) && value)
            {
                RefreshUIAsync().SafeForget();
            }

            async ValueTask RefreshUIAsync()
            {
                try
                {
                    using (await EnterCriticalExecutionAsync().ConfigureAwait(false))
                    {
                        LaunchScheme? scheme = LaunchGameShared.GetCurrentLaunchSchemeFromConfigFile(gameService, infoBarService);

                        await taskContext.SwitchToMainThreadAsync();
                        await SetSelectedSchemeAsync(scheme).ConfigureAwait(true);
                        TrySetGameAccountByDesiredUid();

                        // Try set to the current account.
                        if (SelectedScheme is not null)
                        {
                            // The GameAccount is gaurenteed to be in the view, bacause the scheme is synced
                            SelectedGameAccount ??= gameService.DetectCurrentGameAccount(SelectedScheme);
                        }
                        else
                        {
                            infoBarService.Warning(SH.ViewModelLaunchGameSchemeNotSelected);
                        }
                    }
                }
                catch (UserdataCorruptedException ex)
                {
                    infoBarService.Error(ex);
                }
            }

            void TrySetGameAccountByDesiredUid()
            {
                // Sync uid, almost never hit, so we are not so care about performance
                if (memoryCache.TryRemove(DesiredUid, out object? value) && value is string uid)
                {
                    ArgumentNullException.ThrowIfNull(GameAccountsView);

                    // Exists in the source collection
                    if (GameAccountsView.SourceCollection.Cast<GameAccount>().FirstOrDefault(g => g.AttachUid == uid) is { } sourceAccount)
                    {
                        SelectedGameAccount = GameAccountsView.Cast<GameAccount>().FirstOrDefault(g => g.AttachUid == uid);

                        // But not exists in the view for current scheme
                        if (SelectedGameAccount is null)
                        {
                            infoBarService.Warning(SH.FormatViewModelLaunchGameUnableToSwitchUidAttachedGameAccount(uid, sourceAccount.Name));
                        }
                    }
                }
            }
        }
    }

    public ImmutableList<GamePathEntry> GamePathEntries { get => gamePathEntries; set => SetProperty(ref gamePathEntries, value); }

    [AlsoSets(nameof(GamePathSelectedAndValid))]
    public GamePathEntry? SelectedGamePathEntry
    {
        get => selectedGamePathEntry;
        set
        {
            if (SetProperty(ref selectedGamePathEntry, value, nameof(SelectedGamePathEntry)))
            {
                if (IsViewDisposed)
                {
                    return;
                }

                launchOptions.GamePath = value?.Path ?? string.Empty;
                GamePathSelectedAndValid = File.Exists(launchOptions.GamePath);
            }
        }
    }

    protected override ValueTask<bool> InitializeUIAsync()
    {
        SyncGamePathEntriesAndSelectedGamePathEntryFromLaunchOptions();
        return ValueTask.FromResult(true);
    }

    [Command("SetGamePathCommand")]
    private async Task SetGamePathAsync()
    {
        (bool isOk, string path) = await gameLocatorFactory.LocateAsync(GameLocationSource.Manual).ConfigureAwait(false);
        if (!isOk)
        {
            return;
        }

        await taskContext.SwitchToMainThreadAsync();
        GamePathEntries = launchOptions.UpdateGamePathAndRefreshEntries(path);
    }

    [Command("ResetGamePathCommand")]
    private void ResetGamePath()
    {
        SelectedGamePathEntry = default;
    }

    [Command("RemoveGamePathEntryCommand")]
    private void RemoveGamePathEntry(GamePathEntry? entry)
    {
        GamePathEntries = launchOptions.RemoveGamePathEntry(entry, out GamePathEntry? selected);
        SelectedGamePathEntry = selected;
    }

    [Command("LaunchCommand")]
    private async Task LaunchAsync()
    {
        if (SelectedScheme is null)
        {
            infoBarService.Error(SH.ViewModelLaunchGameSchemeNotSelected);
            return;
        }

        try
        {
            gameService.SetChannelOptions(SelectedScheme);

            LaunchGamePackageConvertDialog dialog = await contentDialogFactory.CreateInstanceAsync<LaunchGamePackageConvertDialog>().ConfigureAwait(false);
            IProgress<PackageReplaceStatus> convertProgress = progressFactory.CreateForMainThread<PackageReplaceStatus>(state => dialog.State = state);

            using (await dialog.BlockAsync(taskContext).ConfigureAwait(false))
            {
                // Always ensure game resources
                if (!await gameService.EnsureGameResourceAsync(SelectedScheme, convertProgress).ConfigureAwait(false))
                {
                    infoBarService.Warning(SH.ViewModelLaunchGameEnsureGameResourceFail, dialog.State.Name);
                    return;
                }
                else
                {
                    await taskContext.SwitchToMainThreadAsync();
                    SyncGamePathEntriesAndSelectedGamePathEntryFromLaunchOptions();
                }
            }

            if (SelectedGameAccount is not null && !gameService.SetGameAccount(SelectedGameAccount))
            {
                infoBarService.Warning(SH.ViewModelLaunchGameSwitchGameAccountFail);
                return;
            }

            IProgress<LaunchStatus> launchProgress = progressFactory.CreateForMainThread<LaunchStatus>(status => launchStatusOptions.LaunchStatus = status);
            await gameService.LaunchAsync(launchProgress).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            if (ex is Win32Exception win32Exception && win32Exception.HResult == HRESULT.E_FAIL)
            {
                // User canceled the operation. ignore
                return;
            }

            logger.LogCritical(ex, "Launch failed");
            infoBarService.Error(ex);
        }
    }

    [Command("DetectGameAccountCommand")]
    private async Task DetectGameAccountAsync()
    {
        try
        {
            if (SelectedScheme is null)
            {
                infoBarService.Error(SH.ViewModelLaunchGameSchemeNotSelected);
                return;
            }

            // If user canceled the operation, the return is null
            if (await gameService.DetectGameAccountAsync(SelectedScheme).ConfigureAwait(false) is { } account)
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
        string game = LaunchOptions.GamePath;
        string? directory = Path.GetDirectoryName(game);
        ArgumentException.ThrowIfNullOrEmpty(directory);
        string screenshot = Path.Combine(directory, "ScreenShot");
        if (Directory.Exists(screenshot))
        {
            await Windows.System.Launcher.LaunchFolderPathAsync(screenshot);
        }
    }

    private async ValueTask SetSelectedSchemeAsync(LaunchScheme? value)
    {
        if (SetProperty(ref selectedScheme, value, nameof(SelectedScheme)))
        {
            UpdateGameResourceAsync(value).SafeForget();
            await UpdateGameAccountsViewAsync().ConfigureAwait(false);

            // Clear the selected game account to prevent setting
            // incorrect CN/OS account when scheme not match
            SelectedGameAccount = default;
        }

        async ValueTask UpdateGameResourceAsync(LaunchScheme? scheme)
        {
            if (scheme is null)
            {
                return;
            }

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

        async ValueTask UpdateGameAccountsViewAsync()
        {
            gameAccountFilter = new(SelectedScheme?.GetSchemeType());
            ObservableCollection<GameAccount> accounts = gameService.GameAccountCollection;

            await taskContext.SwitchToMainThreadAsync();
            GameAccountsView = new(accounts, true)
            {
                Filter = gameAccountFilter.Filter,
            };
        }
    }

    private void SyncGamePathEntriesAndSelectedGamePathEntryFromLaunchOptions()
    {
        GamePathEntries = launchOptions.GetGamePathEntries(out GamePathEntry? entry);
        SelectedGamePathEntry = entry;
    }
}