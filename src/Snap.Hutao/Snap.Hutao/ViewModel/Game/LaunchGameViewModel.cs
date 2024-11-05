﻿// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Messaging;
using Snap.Hutao.Core;
using Snap.Hutao.Core.Database;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service;
using Snap.Hutao.Service.Game;
using Snap.Hutao.Service.Game.Launching;
using Snap.Hutao.Service.Game.Locator;
using Snap.Hutao.Service.Game.PathAbstraction;
using Snap.Hutao.Service.Game.Scheme;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.Service.User;
using Snap.Hutao.UI.Xaml.Data;
using Snap.Hutao.UI.Xaml.View.Window;
using Snap.Hutao.ViewModel.User;
using Snap.Hutao.Web.Hoyolab.HoyoPlay.Connect;
using Snap.Hutao.Web.Hoyolab.HoyoPlay.Connect.Package;
using Snap.Hutao.Web.Response;
using System.Collections.Immutable;
using System.IO;

namespace Snap.Hutao.ViewModel.Game;

[ConstructorGenerated]
[Injection(InjectAs.Singleton)]
internal sealed partial class LaunchGameViewModel : Abstraction.ViewModel, IViewModelSupportLaunchExecution
{
    private readonly GamePackageInstallViewModel gamePackageInstallViewModel;
    private readonly GamePackageViewModel gamePackageViewModel;
    private readonly LaunchStatusOptions launchStatusOptions;
    private readonly IGameLocatorFactory gameLocatorFactory;
    private readonly ILogger<LaunchGameViewModel> logger;
    private readonly LaunchGameShared launchGameShared;
    private readonly IServiceProvider serviceProvider;
    private readonly IInfoBarService infoBarService;
    private readonly IGameServiceFacade gameService;
    private readonly RuntimeOptions runtimeOptions;
    private readonly LaunchOptions launchOptions;
    private readonly IUserService userService;
    private readonly ITaskContext taskContext;
    private readonly AppOptions appOptions;

    private LaunchScheme? selectedScheme;
    private AdvancedCollectionView<GameAccount>? gameAccountsView;
    private GamePackage? gamePackage;
    private bool gamePathSelectedAndValid;
    private ImmutableArray<GamePathEntry> gamePathEntries = [];
    private GamePathEntry? selectedGamePathEntry;
    private GameAccountFilter? gameAccountFilter;

    LaunchGameShared IViewModelSupportLaunchExecution.Shared { get => launchGameShared; }

    public LaunchOptions LaunchOptions { get => launchOptions; }

    public LaunchStatusOptions LaunchStatusOptions { get => launchStatusOptions; }

    public RuntimeOptions RuntimeOptions { get => runtimeOptions; }

    public AppOptions AppOptions { get => appOptions; }

    public GamePackageInstallViewModel GamePackageInstallViewModel { get => gamePackageInstallViewModel; }

    public GamePackageViewModel GamePackageViewModel { get => gamePackageViewModel; }

    public List<LaunchScheme> KnownSchemes { get; } = KnownLaunchSchemes.Get();

    public LaunchScheme? SelectedScheme
    {
        get => selectedScheme;
        set => SetSelectedSchemeAsync(value).SafeForget(logger);
    }

    public AdvancedCollectionView<GameAccount>? GameAccountsView { get => gameAccountsView; set => SetProperty(ref gameAccountsView, value); }

    public GameAccount? SelectedGameAccount { get => GameAccountsView?.CurrentItem; }

    public GamePackage? GamePackage { get => gamePackage; set => SetProperty(ref gamePackage, value); }

    /// <summary>
    /// Update this property will also:
    /// <br/>
    /// 1. Call <see cref="SetSelectedSchemeAsync"/>
    /// <br/>
    /// 2. Force refresh the GamePackageViewModel
    /// <br/>
    /// 3. Try to set the current account for selected scheme
    /// </summary>
    public bool GamePathSelectedAndValid
    {
        get => gamePathSelectedAndValid;
        set
        {
            if (SetProperty(ref gamePathSelectedAndValid, value) && value)
            {
                RefreshUIAsync().SafeForget(logger);
            }

            [SuppressMessage("", "SH003")]
            async Task RefreshUIAsync()
            {
                try
                {
                    using (await EnterCriticalSectionAsync().ConfigureAwait(false))
                    {
                        LaunchScheme? scheme = launchGameShared.GetCurrentLaunchSchemeFromConfigFile();

                        await taskContext.SwitchToMainThreadAsync();
                        await SetSelectedSchemeAsync(scheme).ConfigureAwait(true);

                        await GamePackageViewModel.ForceLoadAsync().ConfigureAwait(false);

                        // Try set to the current account.
                        if (SelectedScheme is not null && GameAccountsView is not null)
                        {
                            // The GameAccount is guaranteed to be in the view, because the scheme is synced
                            GameAccountsView.CurrentItem ??= gameService.DetectCurrentGameAccount(SelectedScheme);
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
        }
    }

    public ImmutableArray<GamePathEntry> GamePathEntries { get => gamePathEntries; set => SetProperty(ref gamePathEntries, value); }

    /// <summary>
    /// Update this property will also:
    /// <br/>
    /// 1. Set the <see cref="LaunchOptions.GamePath"/> to the selected path
    /// <br/>
    /// 2. Update <see cref="GamePathSelectedAndValid"/> to the selected path's existence
    /// </summary>
    public GamePathEntry? SelectedGamePathEntry
    {
        get => selectedGamePathEntry;
        set
        {
            if (!SetProperty(ref selectedGamePathEntry, value))
            {
                return;
            }

            launchOptions.GamePath = value?.Path ?? string.Empty;
            GamePathSelectedAndValid = File.Exists(launchOptions.GamePath);
        }
    }

    public void SetGamePathEntriesAndSelectedGamePathEntry(ImmutableArray<GamePathEntry> gamePathEntries, GamePathEntry? selectedEntry)
    {
        GamePathEntries = gamePathEntries;
        SelectedGamePathEntry = selectedEntry;
    }

    protected override ValueTask<bool> LoadOverrideAsync()
    {
        SetGamePathEntriesAndSelectedGamePathEntry(launchOptions.GetGamePathEntries(out GamePathEntry? entry), entry);
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
        GamePathEntries = launchOptions.UpdateGamePath(path);
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
        UserAndUid? userAndUid = await userService.GetCurrentUserAndUidAsync().ConfigureAwait(false);
        await this.LaunchExecutionAsync(SelectedScheme, userAndUid).ConfigureAwait(false);
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

            if (GameAccountsView is null)
            {
                return;
            }

            // If user canceled the operation, the return is null
            if (await gameService.DetectGameAccountAsync(SelectedScheme).ConfigureAwait(false) is { } account)
            {
                await taskContext.SwitchToMainThreadAsync();
                GameAccountsView.CurrentItem = account;
            }
        }
        catch (Exception ex)
        {
            infoBarService.Error(ex);
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

    [SuppressMessage("", "SH003")]
    private async Task SetSelectedSchemeAsync(LaunchScheme? value)
    {
        if (!SetProperty(ref selectedScheme, value, nameof(SelectedScheme)))
        {
            return;
        }

        // Clear the selected game account to prevent setting
        // incorrect CN/OS account when scheme not match
        if (GameAccountsView is not null)
        {
            GameAccountsView.CurrentItem = default;
        }

        // Update GameAccountsView
        gameAccountFilter = new(SelectedScheme?.GetSchemeType());
        ObservableReorderableDbCollection<GameAccount> accounts = await gameService.GetGameAccountCollectionAsync().ConfigureAwait(false);
        AdvancedCollectionView<GameAccount> accountsView = new(accounts) { Filter = gameAccountFilter.Filter, };

        await taskContext.SwitchToMainThreadAsync();
        GameAccountsView = accountsView;

        if (value is null)
        {
            return;
        }

        await taskContext.SwitchToBackgroundAsync();
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            HoyoPlayClient hoyoPlayClient = scope.ServiceProvider.GetRequiredService<HoyoPlayClient>();
            Response<GamePackagesWrapper> response = await hoyoPlayClient.GetPackagesAsync(value).ConfigureAwait(false);

            if (ResponseValidator.TryValidate(response, serviceProvider, out GamePackagesWrapper? wrapper))
            {
                await taskContext.SwitchToMainThreadAsync();
                GamePackage = wrapper.GamePackages.Single();
            }
        }
    }
}