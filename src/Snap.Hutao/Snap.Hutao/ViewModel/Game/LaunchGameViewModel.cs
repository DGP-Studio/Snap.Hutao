// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Extensions.Caching.Memory;
using Snap.Hutao.Core;
using Snap.Hutao.Core.Database;
using Snap.Hutao.Core.Diagnostics.CodeAnalysis;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Model;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service;
using Snap.Hutao.Service.Game;
using Snap.Hutao.Service.Game.Locator;
using Snap.Hutao.Service.Game.PathAbstraction;
using Snap.Hutao.Service.Game.Scheme;
using Snap.Hutao.Service.Game.Unlocker;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.Service.User;
using Snap.Hutao.UI.Xaml.Data;
using Snap.Hutao.Web.Hoyolab.HoyoPlay.Connect;
using Snap.Hutao.Web.Hoyolab.HoyoPlay.Connect.Package;
using System.Collections.Immutable;
using System.IO;

namespace Snap.Hutao.ViewModel.Game;

/// <summary>
/// 启动游戏视图模型
/// </summary>
[HighQuality]
[ConstructorGenerated]
[Injection(InjectAs.Singleton)]
internal sealed partial class LaunchGameViewModel : Abstraction.ViewModel, IViewModelSupportLaunchExecution
{
    /// <summary>
    /// 启动游戏目标 Uid
    /// </summary>
    public const string DesiredUid = nameof(DesiredUid);

    private readonly LaunchStatusOptions launchStatusOptions;
    private readonly IGameLocatorFactory gameLocatorFactory;
    private readonly LaunchGameShared launchGameShared;
    private readonly IInfoBarService infoBarService;
    private readonly IGameServiceFacade gameService;
    private readonly RuntimeOptions runtimeOptions;
    private readonly HoyoPlayClient hoyoPlayClient;
    private readonly LaunchOptions launchOptions;
    private readonly IUserService userService;
    private readonly ITaskContext taskContext;
    private readonly IMemoryCache memoryCache;
    private readonly AppOptions appOptions;

    private LaunchScheme? selectedScheme;
    private AdvancedCollectionView<GameAccount>? gameAccountsView;
    private GameAccount? selectedGameAccount;
    private GamePackage? gamePackage;
    private bool gamePathSelectedAndValid;
    private ImmutableList<GamePathEntry> gamePathEntries;
    private GamePathEntry? selectedGamePathEntry;
    private GameAccountFilter? gameAccountFilter;

    LaunchGameShared IViewModelSupportLaunchExecution.Shared { get => launchGameShared; }

    public LaunchOptions LaunchOptions { get => launchOptions; }

    public LaunchStatusOptions LaunchStatusOptions { get => launchStatusOptions; }

    public RuntimeOptions RuntimeOptions { get => runtimeOptions; }

    public AppOptions AppOptions { get => appOptions; }

    public List<LaunchScheme> KnownSchemes { get; } = KnownLaunchSchemes.Get();

    public LaunchScheme? SelectedScheme
    {
        get => selectedScheme;
        set => SetSelectedSchemeAsync(value).SafeForget();
    }

    public AdvancedCollectionView<GameAccount>? GameAccountsView { get => gameAccountsView; set => SetProperty(ref gameAccountsView, value); }

    public GameAccount? SelectedGameAccount { get => selectedGameAccount; set => SetProperty(ref selectedGameAccount, value); }

    public GamePackage? GamePackage { get => gamePackage; set => SetProperty(ref gamePackage, value); }

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
                        LaunchScheme? scheme = launchGameShared.GetCurrentLaunchSchemeFromConfigFile();

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
                catch (HutaoException ex)
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
                    if (GameAccountsView.SourceCollection.FirstOrDefault(g => g.AttachUid == uid) is { } sourceAccount)
                    {
                        SelectedGameAccount = GameAccountsView.View.FirstOrDefault(g => g.AttachUid == uid);

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

    public void SetGamePathEntriesAndSelectedGamePathEntry(ImmutableList<GamePathEntry> gamePathEntries, GamePathEntry? selectedEntry)
    {
        GamePathEntries = gamePathEntries;
        SelectedGamePathEntry = selectedEntry;
    }

    protected override ValueTask<bool> InitializeUIAsync()
    {
        ImmutableList<GamePathEntry> gamePathEntries = launchOptions.GetGamePathEntries(out GamePathEntry? entry);
        SetGamePathEntriesAndSelectedGamePathEntry(gamePathEntries, entry);
        return ValueTask.FromResult(true);
    }

    [Command("IdentifyMonitorsCommand")]
    private static async Task IdentifyMonitorsAsync()
    {
        await IdentifyMonitorWindow.IdentifyAllMonitorsAsync(3).ConfigureAwait(false);
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
        await this.LaunchExecutionAsync(SelectedScheme).ConfigureAwait(false);
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
        catch (Exception ex)
        {
            infoBarService.Error(ex);
        }
    }

    [Command("AttachGameAccountCommand")]
    private void AttachGameAccountToCurrentUserGameRole(GameAccount? gameAccount)
    {
        if (gameAccount is null)
        {
            return;
        }

        if (userService.Current?.SelectedUserGameRole is { } role)
        {
            gameService.AttachGameAccountToUid(gameAccount, role.GameUid);
        }
        else
        {
            infoBarService.Warning(SH.MustSelectUserAndUid);
        }
    }

    [Command("ModifyGameAccountCommand")]
    private async Task ModifyGameAccountAsync(GameAccount? gameAccount)
    {
        if (gameAccount is null)
        {
            return;
        }

        await gameService.ModifyGameAccountAsync(gameAccount).ConfigureAwait(false);
    }

    [Command("RemoveGameAccountCommand")]
    private async Task RemoveGameAccountAsync(GameAccount? gameAccount)
    {
        if (gameAccount is null)
        {
            return;
        }

        await gameService.RemoveGameAccountAsync(gameAccount).ConfigureAwait(false);
    }

    [Command("OpenScreenshotFolderCommand")]
    private async Task OpenScreenshotFolderAsync()
    {
        if (!launchOptions.TryGetGameFileSystem(out GameFileSystem? gameFileSystem))
        {
            return;
        }

        Directory.CreateDirectory(gameFileSystem.ScreenShotDirectory);
        await Windows.System.Launcher.LaunchFolderPathAsync(gameFileSystem.ScreenShotDirectory);
    }

    private async ValueTask SetSelectedSchemeAsync(LaunchScheme? value)
    {
        if (SetProperty(ref selectedScheme, value, nameof(SelectedScheme)))
        {
            // Clear the selected game account to prevent setting
            // incorrect CN/OS account when scheme not match
            SelectedGameAccount = default;

            await UpdateGameAccountsViewAsync().ConfigureAwait(false);
            UpdateGamePackageAsync(value).SafeForget();
        }

        async ValueTask UpdateGamePackageAsync(LaunchScheme? scheme)
        {
            if (scheme is null)
            {
                return;
            }

            await taskContext.SwitchToBackgroundAsync();
            Web.Response.Response<GamePackagesWrapper> response = await hoyoPlayClient
                .GetPackagesAsync(scheme)
                .ConfigureAwait(false);

            if (response.IsOk())
            {
                await taskContext.SwitchToMainThreadAsync();
                GamePackage = response.Data.GamePackages.Single();
            }
        }

        async ValueTask UpdateGameAccountsViewAsync()
        {
            gameAccountFilter = new(SelectedScheme?.GetSchemeType());
            ObservableReorderableDbCollection<GameAccount> accounts = gameService.GameAccountCollection;

            await taskContext.SwitchToMainThreadAsync();
            GameAccountsView = new(accounts, true)
            {
                Filter = gameAccountFilter.Filter,
            };
        }
    }
}