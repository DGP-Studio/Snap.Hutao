// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Core.Logging;
using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.Model;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Service.Game;
using Snap.Hutao.Service.Game.FileSystem;
using Snap.Hutao.Service.Game.Locator;
using Snap.Hutao.Service.Game.PathAbstraction;
using Snap.Hutao.Service.Game.Scheme;
using Snap.Hutao.Service.Navigation;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.Service.User;
using Snap.Hutao.UI.Input.LowLevel;
using Snap.Hutao.UI.Xaml.Data;
using Snap.Hutao.UI.Xaml.View.Dialog;
using Snap.Hutao.UI.Xaml.View.Window;
using Snap.Hutao.ViewModel.User;
using System.Collections.Immutable;
using System.IO;

namespace Snap.Hutao.ViewModel.Game;

[ConstructorGenerated]
[Service(ServiceLifetime.Singleton)]
internal sealed partial class LaunchGameViewModel : Abstraction.ViewModel, IViewModelSupportLaunchExecution, INavigationRecipient
{
    private readonly IGameLocatorFactory gameLocatorFactory;
    private readonly IServiceProvider serviceProvider;
    private readonly IGameService gameService;
    private readonly IUserService userService;
    private readonly ITaskContext taskContext;
    private readonly IMessenger messenger;

    // Required for the SetProperty
    private LaunchScheme? selectedScheme;

    public partial GamePackageInstallViewModel GamePackageInstallViewModel { get; }

    public partial GamePackageViewModel GamePackageViewModel { get; }

    public partial LaunchStatusOptions LaunchStatusOptions { get; }

    public partial LowLevelKeyOptions LowLevelKeyOptions { get; }

    public partial LaunchOptions LaunchOptions { get; }

    public partial LaunchGameShared Shared { get; }

    public ImmutableArray<LaunchScheme> KnownSchemes { get; } = KnownLaunchSchemes.Values;

    public LaunchScheme? SelectedScheme
    {
        get => selectedScheme;
        set => SetSelectedSchemeAsync(value).SafeForget();
    }

    [field: MaybeNull]
    public IObservableProperty<NameValue<PlatformType>?> SelectedPlatformType { get => field ??= LaunchOptions.PlatformType.AsNameValue(LaunchOptions.PlatformTypes); }

    public IAdvancedCollectionView<GameAccount>? GameAccountsView { get; set => SetProperty(ref field, value); }

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
                RefreshUIAsync().SafeForget();
            }

            [SuppressMessage("", "SH003")]
            async Task RefreshUIAsync()
            {
                try
                {
                    using (await EnterCriticalSectionAsync().ConfigureAwait(false))
                    {
                        LaunchScheme? scheme = Shared.GetCurrentLaunchSchemeFromConfigurationFile();

                        await taskContext.SwitchToMainThreadAsync();
                        await SetSelectedSchemeAsync(scheme).ConfigureAwait(true);

                        await GamePackageViewModel.ForceLoadAsync().ConfigureAwait(true);

                        // Try set to the current account.
                        if (SelectedScheme is not null && GameAccountsView is not null)
                        {
                            // The GameAccount is guaranteed to be in the view, because the scheme is synced
                            // Except when scheme is bilibili, which is not supported
                            _ = GameAccountsView.CurrentItem is null && GameAccountsView.MoveCurrentTo(gameService.DetectCurrentGameAccountNoThrow(SelectedScheme));
                        }
                        else
                        {
                            messenger.Send(InfoBarMessage.Warning(SH.ViewModelLaunchGameSchemeNotSelected));
                        }
                    }
                }
                catch (HutaoException ex)
                {
                    messenger.Send(InfoBarMessage.Error(ex));
                }
            }
        }
    }

    /// <summary>
    /// Update this property will also:
    /// <br/>
    /// 1. Set the <see cref="LaunchOptions.GamePath"/> to the selected path
    /// <br/>
    /// 2. Update <see cref="GamePathSelectedAndValid"/> to the selected path's existence
    /// </summary>
    [Obsolete]
    public GamePathEntry? SelectedGamePathEntry
    {
        get;
        set
        {
            if (value is not null && !LaunchOptions.GamePathEntries.Value.Contains(value))
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
                    LaunchOptions.GamePath.Value = value?.Path ?? string.Empty;
                }
            }

            GamePathSelectedAndValid = File.Exists(LaunchOptions.GamePath.Value);
        }
    }

    public bool CanResetGamePathEntry { get; set => SetProperty(ref field, value); } = !GameLifeCycle.IsGameRunning();

    public async ValueTask<bool> ReceiveAsync(INavigationExtraData data, CancellationToken token)
    {
        if (!await Initialization.Task.ConfigureAwait(false))
        {
            return false;
        }

        if (data is LaunchGameExtraData { TypedData: { } uid })
        {
            return await userService.SetCurrentUserByUidAsync(uid).ConfigureAwait(false);
        }

        return false;
    }

    protected override async ValueTask<bool> LoadOverrideAsync(CancellationToken token)
    {
        if (LaunchOptions.GamePathEntries.Value.IsDefaultOrEmpty)
        {
            await serviceProvider.GetRequiredService<IGamePathService>().SilentLocateAllGamePathAsync().ConfigureAwait(false);
        }

        Shared.ResumeLaunchExecutionAsync(this).SafeForget();

        return true;
    }

    [Command("IdentifyMonitorsCommand")]
    private static async Task IdentifyMonitorsAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Identify monitors", "LaunchGameViewModel.Command"));
        await IdentifyMonitorWindow.IdentifyAllMonitorsAsync(3).ConfigureAwait(false);
    }

    [Command("SetGamePathCommand")]
    private async Task SetGamePathAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Set game path by picker", "LaunchGameViewModel.Command"));
        if (await gameLocatorFactory.LocateSingleAsync(GameLocationSourceKind.Manual).ConfigureAwait(false) is not (true, var path))
        {
            return;
        }

        await taskContext.SwitchToMainThreadAsync();
        LaunchOptions.UpdateGamePath(path);
    }

    [Command("ResetGamePathCommand")]
    private void ResetGamePath()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Reset game path", "LaunchGameViewModel.Command"));
        SelectedGamePathEntry = default;
    }

    [Command("RemoveGamePathEntryCommand")]
    private void RemoveGamePathEntry(GamePathEntry? entry)
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Remove game path", "LaunchGameViewModel.Command"));
        LaunchOptions.RemoveGamePathEntry(entry, out GamePathEntry? selected);
        SelectedGamePathEntry = selected;
    }

    [Command("RemoveAspectRatioCommand")]
    private void RemoveAspectRatio(AspectRatio? aspectRatio)
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Remove aspect ratio", "LaunchGameViewModel.Command"));
        if (aspectRatio is null)
        {
            return;
        }

        if (aspectRatio.Equals(LaunchOptions.SelectedAspectRatio))
        {
            LaunchOptions.SelectedAspectRatio = default;
        }

        LaunchOptions.AspectRatios.Remove(aspectRatio);
    }

    [Command("LaunchCommand")]
    private async Task LaunchAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Launch game", "LaunchGameViewModel.Command"));

        UserAndUid? userAndUid = await userService.GetCurrentUserAndUidAsync().ConfigureAwait(false);
        await Shared.DefaultLaunchExecutionAsync(this, userAndUid).ConfigureAwait(false);
    }

    [Command("DetectGameAccountCommand")]
    private async Task DetectGameAccountAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Detect registry game account", "LaunchGameViewModel.Command"));

        try
        {
            if (SelectedScheme is null)
            {
                messenger.Send(InfoBarMessage.Error(SH.ViewModelLaunchGameSchemeNotSelected));
                return;
            }

            if (GameAccountsView is null)
            {
                return;
            }

            GameAccount? currentAccount = await gameService.DetectGameAccountAsync(SelectedScheme, async () =>
            {
                using (IServiceScope scope = serviceProvider.CreateScope())
                {
                    LaunchGameAccountNameDialog dialog = await scope.ServiceProvider
                        .GetRequiredService<IContentDialogFactory>()
                        .CreateInstanceAsync<LaunchGameAccountNameDialog>(scope.ServiceProvider)
                        .ConfigureAwait(false);
                    return await dialog.GetInputNameAsync().ConfigureAwait(false);
                }
            }).ConfigureAwait(false);

            if (currentAccount is not null)
            {
                await taskContext.SwitchToMainThreadAsync();
                GameAccountsView.MoveCurrentTo(currentAccount);
            }
        }
        catch (Exception ex)
        {
            messenger.Send(InfoBarMessage.Error(ex));
        }
    }

    [Command("ModifyGameAccountCommand")]
    private async Task ModifyGameAccountAsync(GameAccount? gameAccount)
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Modify registry game account", "LaunchGameViewModel.Command"));

        if (gameAccount is null)
        {
            return;
        }

        await gameService.ModifyGameAccountAsync(gameAccount, async originalName =>
        {
            using (IServiceScope scope = serviceProvider.CreateScope())
            {
                LaunchGameAccountNameDialog dialog = await scope.ServiceProvider
                    .GetRequiredService<IContentDialogFactory>()
                    .CreateInstanceAsync<LaunchGameAccountNameDialog>(scope.ServiceProvider, originalName)
                    .ConfigureAwait(false);

                return await dialog.GetInputNameAsync().ConfigureAwait(false);
            }
        }).ConfigureAwait(false);
    }

    [Command("RemoveGameAccountCommand")]
    private async Task RemoveGameAccountAsync(GameAccount? gameAccount)
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Remove registry game account", "LaunchGameViewModel.Command"));

        if (gameAccount is null)
        {
            return;
        }

        await gameService.RemoveGameAccountAsync(gameAccount).ConfigureAwait(false);
    }

    [Command("OpenScreenshotFolderCommand")]
    private async Task OpenScreenshotFolderAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Open screenshot folder", "LaunchGameViewModel.Command"));

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

    [Command("KillGameProcessCommand")]
    private void KillGameProcess()
    {
        if (!LaunchOptions.CanKillGameProcess.Value)
        {
            return;
        }

        gameService.KillGameProcess();
    }

    [SuppressMessage("", "SH003")]
    private async Task SetSelectedSchemeAsync(LaunchScheme? value)
    {
        if (!SetProperty(ref selectedScheme, value, nameof(SelectedScheme)))
        {
            return;
        }

        if (GameAccountsView is null)
        {
            IAdvancedCollectionView<GameAccount> accountsView = await gameService.GetGameAccountCollectionAsync().ConfigureAwait(true);
            await taskContext.SwitchToMainThreadAsync();
            GameAccountsView = accountsView;
        }
        else
        {
            // Clear the selected game account to prevent setting
            // incorrect CN/OS registry account when scheme not match
            await taskContext.SwitchToMainThreadAsync();
            GameAccountsView.MoveCurrentTo(default);
        }

        // Update GameAccountsView
        await taskContext.SwitchToMainThreadAsync();
        GameAccountsView.Filter = GameAccountFilter.Create(SelectedScheme?.GetSchemeType());
    }
}