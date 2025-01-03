// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Messaging;
using Snap.Hutao.Core;
using Snap.Hutao.Core.Database;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Game;
using Snap.Hutao.Service.Game.Launching;
using Snap.Hutao.Service.Game.Locator;
using Snap.Hutao.Service.Game.PathAbstraction;
using Snap.Hutao.Service.Game.Scheme;
using Snap.Hutao.Service.Navigation;
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
internal sealed partial class LaunchGameViewModel : Abstraction.ViewModel, IViewModelSupportLaunchExecution, INavigationRecipient,
    IRecipient<LaunchExecutionGameFileSystemExclusiveAccessChangedMessage>
{
    private readonly IGameLocatorFactory gameLocatorFactory;
    private readonly ILogger<LaunchGameViewModel> logger;
    private readonly LaunchGameShared launchGameShared;
    private readonly IServiceProvider serviceProvider;
    private readonly IInfoBarService infoBarService;
    private readonly IGameService gameService;
    private readonly IUserService userService;
    private readonly ITaskContext taskContext;

    private LaunchScheme? selectedScheme;
    private GameAccountFilter? gameAccountFilter;

    LaunchGameShared IViewModelSupportLaunchExecution.Shared { get => launchGameShared; }

    public partial LaunchOptions LaunchOptions { get; }

    public partial LaunchStatusOptions LaunchStatusOptions { get; }

    public partial RuntimeOptions RuntimeOptions { get; }

    public partial GamePackageInstallViewModel GamePackageInstallViewModel { get; }

    public partial GamePackageViewModel GamePackageViewModel { get; }

    public ImmutableArray<LaunchScheme> KnownSchemes { get; } = KnownLaunchSchemes.Values;

    public LaunchScheme? SelectedScheme
    {
        get => selectedScheme;
        set => SetSelectedSchemeAsync(value).SafeForget(logger);
    }

    public AdvancedCollectionView<GameAccount>? GameAccountsView { get; set => SetProperty(ref field, value); }

    public GameAccount? SelectedGameAccount { get => GameAccountsView?.CurrentItem; }

    public GamePackage? GamePackage { get; set => SetProperty(ref field, value); }

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
        get;
        set
        {
            if (SetProperty(ref field, value) && value)
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

                        await GamePackageViewModel.ForceLoadAsync().ConfigureAwait(true);

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

    public ImmutableArray<GamePathEntry> GamePathEntries { get; set => SetProperty(ref field, value); } = [];

    public ImmutableArray<AspectRatio> AspectRatios { get; set => SetProperty(ref field, value); } = [];

    /// <summary>
    /// Update this property will also:
    /// <br/>
    /// 1. Set the <see cref="LaunchOptions.GamePath"/> to the selected path
    /// <br/>
    /// 2. Update <see cref="GamePathSelectedAndValid"/> to the selected path's existence
    /// </summary>
    public GamePathEntry? SelectedGamePathEntry
    {
        get;
        set
        {
            if (value is not null && !LaunchOptions.GamePathEntries.Contains(value))
            {
                HutaoException.InvalidOperation("Selected game path entry is not in the game path entries.");
            }

            if (!SetProperty(ref field, value))
            {
                return;
            }

            // We are selecting from existing entries, so we don't need to update GamePathEntries
            if (LaunchOptions.GamePathLock.TryWriterLock(out AsyncReaderWriterLock.Releaser releaser))
            {
                using (releaser)
                {
                    LaunchOptions.GamePath = value?.Path ?? string.Empty;
                }
            }

            GamePathSelectedAndValid = File.Exists(LaunchOptions.GamePath);
        }
    }

    public bool CanResetGamePathEntry { get; set => SetProperty(ref field, value); } = true;

    public void SetGamePathEntriesAndSelectedGamePathEntry(ImmutableArray<GamePathEntry> gamePathEntries, GamePathEntry? selectedEntry)
    {
        GamePathEntries = gamePathEntries;
        SelectedGamePathEntry = selectedEntry;
    }

    public async ValueTask<bool> ReceiveAsync(INavigationExtraData data)
    {
        if (!await Initialization.Task.ConfigureAwait(false))
        {
            return false;
        }

        if (data is LaunchGameWithUidData { TypedData: { } uid })
        {
            return await userService.SetCurrentUserByUidAsync(uid).ConfigureAwait(false);
        }

        return false;
    }

    public void Receive(LaunchExecutionGameFileSystemExclusiveAccessChangedMessage message)
    {
        taskContext.BeginInvokeOnMainThread(() => CanResetGamePathEntry = message.CanAccess);
    }

    protected override async ValueTask<bool> LoadOverrideAsync()
    {
        if (LaunchOptions.GamePathEntries.IsDefaultOrEmpty)
        {
            await serviceProvider.GetRequiredService<IGamePathService>().SilentLocateAllGamePathAsync().ConfigureAwait(false);
        }

        await taskContext.SwitchToMainThreadAsync();
        this.SetGamePathEntriesAndSelectedGamePathEntry(LaunchOptions);
        AspectRatios = LaunchOptions.AspectRatios;
        return true;
    }

    [Command("IdentifyMonitorsCommand")]
    private static async Task IdentifyMonitorsAsync()
    {
        await IdentifyMonitorWindow.IdentifyAllMonitorsAsync(3).ConfigureAwait(false);
    }

    [Command("SetGamePathCommand")]
    private async Task SetGamePathAsync()
    {
        (bool isOk, string path) = await gameLocatorFactory.LocateSingleAsync(GameLocationSourceKind.Manual).ConfigureAwait(false);
        if (!isOk)
        {
            return;
        }

        await taskContext.SwitchToMainThreadAsync();
        GamePathEntries = LaunchOptions.UpdateGamePath(path);
    }

    [Command("ResetGamePathCommand")]
    private void ResetGamePath()
    {
        SelectedGamePathEntry = default;
    }

    [Command("RemoveGamePathEntryCommand")]
    private void RemoveGamePathEntry(GamePathEntry? entry)
    {
        GamePathEntries = LaunchOptions.RemoveGamePathEntry(entry, out GamePathEntry? selected);
        SelectedGamePathEntry = selected;
    }

    [Command("RemoveAspectRatioCommand")]
    private void RemoveAspectRatio(AspectRatio aspectRatio)
    {
        AspectRatios = LaunchOptions.RemoveAspectRatio(aspectRatio);
    }

    [Command("LaunchCommand")]
    private async Task LaunchAsync()
    {
        UserAndUid? userAndUid = await userService.GetCurrentUserAndUidAsync().ConfigureAwait(false);
        await this.LaunchExecutionAsync(SelectedScheme, userAndUid).ConfigureAwait(false);

        // AspectRatios might be updated during the launch
        await taskContext.SwitchToMainThreadAsync();
        AspectRatios = LaunchOptions.AspectRatios;
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
        if (!LaunchOptions.TryGetGameFileSystem(out IGameFileSystem? gameFileSystem))
        {
            return;
        }

        using (gameFileSystem)
        {
            Directory.CreateDirectory(gameFileSystem.GetScreenShotDirectory());
            await Windows.System.Launcher.LaunchFolderPathAsync(gameFileSystem.GetScreenShotDirectory());
        }
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