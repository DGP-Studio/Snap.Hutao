// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Core.Logging;
using Snap.Hutao.Core.Property;
using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.Model;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Service.Game;
using Snap.Hutao.Service.Game.FileSystem;
using Snap.Hutao.Service.Game.Locator;
using Snap.Hutao.Service.Game.Package;
using Snap.Hutao.Service.Game.PathAbstraction;
using Snap.Hutao.Service.Game.Scheme;
using Snap.Hutao.Service.Navigation;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.Service.User;
using Snap.Hutao.UI.Input.LowLevel;
using Snap.Hutao.UI.Xaml.View.Dialog;
using Snap.Hutao.UI.Xaml.View.Window;
using Snap.Hutao.ViewModel.User;
using System.Collections.Immutable;
using System.IO;

namespace Snap.Hutao.ViewModel.Game;

[BindableCustomPropertyProvider]
[GeneratedConstructor]
[Service(ServiceLifetime.Singleton)]
internal sealed partial class LaunchGameViewModel : Abstraction.ViewModel, IViewModelSupportLaunchExecution, INavigationRecipient
{
    private readonly IGameLocatorFactory gameLocatorFactory;
    private readonly IServiceProvider serviceProvider;
    private readonly IGameService gameService;
    private readonly IUserService userService;
    private readonly ITaskContext taskContext;
    private readonly IMessenger messenger;

    public partial GamePackageInstallViewModel GamePackageInstallViewModel { get; }

    public partial GamePackageViewModel GamePackageViewModel { get; }

    public partial LaunchStatusOptions LaunchStatusOptions { get; }

    public partial LowLevelKeyOptions LowLevelKeyOptions { get; }

    public partial LaunchOptions LaunchOptions { get; }

    public partial LaunchGameShared Shared { get; }

    public ImmutableArray<LaunchScheme> KnownSchemes { get; } = KnownLaunchSchemes.Values;

    LaunchScheme? IViewModelSupportLaunchExecution.TargetScheme { get => TargetSchemeFilteredGameAccountsView.Scheme; }

    LaunchScheme? IViewModelSupportLaunchExecution.CurrentScheme { get => Shared.GetCurrentLaunchSchemeFromConfigurationFile(); }

    GameAccount? IViewModelSupportLaunchExecution.GameAccount { get => TargetSchemeFilteredGameAccountsView.View?.CurrentItem; }

    public LaunchSchemeFilteredGameAccountsView TargetSchemeFilteredGameAccountsView { get => field ??= new(IsViewUnloaded, gameService, taskContext, messenger); private set; }

    public IObservableProperty<NameValue<PlatformType>?> SelectedPlatformType { get => field ??= LaunchOptions.PlatformType.AsNameValue(LaunchOptions.PlatformTypes); }

    public IObservableProperty<GamePathEntry?> GamePathEntry { get => field ??= LaunchOptions.GamePathEntry.SetWithCondition(static (value, unloaded) => !unloaded.Value && value is not null, IsViewUnloaded); }

    public IReadOnlyObservableProperty<string> DisplayGamePath { get => field ??= Property.Observe(LaunchOptions.GamePathEntry, static entry => SH.FormatViewModelLaunchGameDisplayGamePath(entry?.Path)); }

    public IReadOnlyObservableProperty<bool> GamePathEntryValid { get => field ??= Property.Observe(LaunchOptions.GamePathEntry, static entry => !string.IsNullOrEmpty(entry?.Path)).WithValueChangedCallback(static (v, vm) => vm.HandleGamePathEntryChangeAsync().SafeForget(), this); }

    public IReadOnlyObservableProperty<bool> IsIslandConnected { get => GameLifeCycle.IsIslandConnected.AsReadOnly(); }

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

    [SuppressMessage("", "SH003")]
    public async Task HandleGamePathEntryChangeAsync()
    {
        try
        {
            using (await EnterCriticalSectionAsync().ConfigureAwait(false))
            {
                LaunchScheme? currentScheme = GamePathEntry.Value is not null
                    ? Shared.GetCurrentLaunchSchemeFromConfigurationFile()
                    : default;

                await taskContext.SwitchToMainThreadAsync();
                await TargetSchemeFilteredGameAccountsView.SetAsync(currentScheme).ConfigureAwait(true);
                await GamePackageViewModel.ReloadAsync().ConfigureAwait(true);
            }
        }
        catch (HutaoException ex)
        {
            messenger.Send(InfoBarMessage.Error(ex));
        }
    }

    ValueTask<BlockDeferral<PackageConvertStatus>> IViewModelSupportLaunchExecution.CreateConvertBlockDeferralAsync()
    {
        return BlockDeferral<PackageConvertStatus>.CreateAsync<LaunchGamePackageConvertDialog>(serviceProvider, static (state, dialog) => dialog.State = state);
    }

    protected override async ValueTask<bool> LoadOverrideAsync(CancellationToken token)
    {
        if (LaunchOptions.GamePathEntries.Value.IsDefaultOrEmpty)
        {
            await serviceProvider.GetRequiredService<IGamePathService>().SilentLocateAllGamePathAsync().ConfigureAwait(false);
        }

        await HandleGamePathEntryChangeAsync().ConfigureAwait(false);
        Shared.ResumeLaunchExecutionAsync(this).SafeForget();
        return true;
    }

    [Command("IdentifyMonitorsCommand")]
    private static async Task IdentifyMonitorsAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Identify monitors", "LaunchGameViewModel.Command"));
        await IdentifyMonitorWindow.IdentifyAllMonitorsAsync(3).ConfigureAwait(false);
    }

    [Command("PickGamePathCommand")]
    private async Task PickGamePathAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Set game path by picker", "LaunchGameViewModel.Command"));
        if (await gameLocatorFactory.LocateSingleAsync(GameLocationSourceKind.Manual).ConfigureAwait(false) is not (true, var path))
        {
            return;
        }

        await taskContext.SwitchToMainThreadAsync();
        LaunchOptions.PerformGamePathEntrySynchronization(path);
    }

    [Command("ResetGamePathCommand")]
    private void ResetGamePath()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Reset game path", "LaunchGameViewModel.Command"));
        LaunchOptions.GamePathEntry.Value = default;
        _ = 1;
    }

    [Command("RemoveGamePathEntryCommand")]
    private void RemoveGamePathEntry(GamePathEntry? entry)
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Remove game path", "LaunchGameViewModel.Command"));
        LaunchOptions.RemoveGamePathEntry(entry);
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

    [Command("ConvertCommand")]
    private async Task ConvertAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Convert game server", "LaunchGameViewModel.Command"));
        await Shared.ConvertLaunchExecutionAsync(this).ConfigureAwait(false);
    }

    [Command("DetectGameAccountCommand")]
    private async Task DetectGameAccountAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Detect registry game account", "LaunchGameViewModel.Command"));

        try
        {
            if (TargetSchemeFilteredGameAccountsView.Scheme is null)
            {
                messenger.Send(InfoBarMessage.Error(SH.ViewModelLaunchGameSchemeNotSelected));
                return;
            }

            if (TargetSchemeFilteredGameAccountsView.View is null)
            {
                return;
            }

            GameAccount? currentAccount = await gameService.DetectGameAccountAsync(TargetSchemeFilteredGameAccountsView.Scheme, async (suggestedName) =>
            {
                using (IServiceScope scope = serviceProvider.CreateScope())
                {
                    LaunchGameAccountNameDialog dialog = await scope.ServiceProvider
                        .GetRequiredService<IContentDialogFactory>()
                        .CreateInstanceAsync<LaunchGameAccountNameDialog>(scope.ServiceProvider, suggestedName)
                        .ConfigureAwait(false);
                    return await dialog.GetInputNameAsync().ConfigureAwait(false);
                }
            }).ConfigureAwait(false);

            if (currentAccount is not null)
            {
                await taskContext.SwitchToMainThreadAsync();
                TargetSchemeFilteredGameAccountsView.View.MoveCurrentTo(currentAccount);
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

        const string LockTrace = $"{nameof(LaunchGameViewModel)}.{nameof(OpenScreenshotFolderAsync)}";
        if (LaunchOptions.TryGetGameFileSystem(LockTrace, out IGameFileSystem? gameFileSystem) is not GameFileSystemErrorKind.None)
        {
            return;
        }

        ArgumentNullException.ThrowIfNull(gameFileSystem);
        using (gameFileSystem)
        {
            Directory.CreateDirectory(gameFileSystem.GetScreenShotDirectory());
            await Windows.System.Launcher.LaunchFolderPathAsync(gameFileSystem.GetScreenShotDirectory());
        }
    }

    [Command("KillGameProcessCommand")]
    private async Task KillGameProcess()
    {
        if (!LaunchOptions.CanKillGameProcess.Value)
        {
            return;
        }

        await GameLifeCycle.TryKillGameProcessAsync(taskContext).ConfigureAwait(false);
    }
}