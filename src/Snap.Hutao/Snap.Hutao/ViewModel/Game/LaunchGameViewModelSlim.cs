// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Snap.Hutao.Core;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Game;
using Snap.Hutao.Service.Game.Launching;
using Snap.Hutao.Service.Game.Launching.Handler;
using Snap.Hutao.Service.Game.Scheme;
using Snap.Hutao.Service.Game.Unlocker;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.Service.User;
using Snap.Hutao.UI.Xaml.Data;
using Snap.Hutao.UI.Xaml.View.Page;
using Snap.Hutao.ViewModel.User;
using System.Diagnostics;

namespace Snap.Hutao.ViewModel.Game;

[Injection(InjectAs.Transient)]
[ConstructorGenerated(CallBaseConstructor = true)]
internal sealed partial class LaunchGameViewModelSlim : Abstraction.ViewModelSlim<LaunchGamePage>, IViewModelSupportLaunchExecution
{
    private readonly ILogger<LaunchGameViewModelSlim> logger;
    private readonly IServiceProvider serviceProvider;
    private readonly IInfoBarService infoBarService;
    private readonly IGameService gameService;
    private readonly IUserService userService;
    private readonly ITaskContext taskContext;

    public partial LaunchGameShared Shared { get; }

    public partial LaunchStatusOptions LaunchStatusOptions { get; }

    public partial LaunchOptions LaunchOptions { get; }

    [ObservableProperty]
    public partial IAdvancedCollectionView<GameAccount>? GameAccountsView { get; set; }

    public LaunchScheme? SelectedScheme { get => Shared.GetCurrentLaunchSchemeFromConfigFile(); }

    public GameAccount? SelectedGameAccount { get => GameAccountsView?.CurrentItem; }

    protected override async Task LoadAsync()
    {
        Shared.ResumeLaunchExecutionAsync().SafeForget(logger);

        LaunchScheme? scheme = Shared.GetCurrentLaunchSchemeFromConfigFile();
        IAdvancedCollectionView<GameAccount> accountsView = await gameService.GetGameAccountCollectionAsync().ConfigureAwait(false);
        accountsView.Filter = GameAccountFilter.CreateFilter(scheme?.GetSchemeType());
        await taskContext.SwitchToMainThreadAsync();
        GameAccountsView = accountsView;

        try
        {
            if (scheme is not null)
            {
                // Try set to the current account.
                await taskContext.SwitchToMainThreadAsync();
                if (GameAccountsView.CurrentItem is null)
                {
                    GameAccountsView.MoveCurrentTo(gameService.DetectCurrentGameAccount(scheme));
                }
            }
        }
        catch (Exception ex)
        {
            infoBarService.Error(ex);
        }
    }

    [Command("LaunchCommand")]
    private async Task LaunchAsync()
    {
        UserAndUid? userAndUid = await userService.GetCurrentUserAndUidAsync().ConfigureAwait(false);
        await this.LaunchExecutionAsync(userAndUid).ConfigureAwait(false);
    }

    private async ValueTask ResumeLaunchExecutionAsync(CancellationToken token)
    {
        await Task.CompletedTask.ConfigureAwait(ConfigureAwaitOptions.ForceYielding);
        if (!LaunchExecutionEnsureGameNotRunningHandler.IsGameRunning())
        {
            return;
        }

        unsafe
        {
            SpinWaitPolyfill.SpinWhile(&LaunchExecutionEnsureGameNotRunningHandler.IsGameRunning);
        }

        if (token.IsCancellationRequested)
        {
            return;
        }

        serviceProvider.GetRequiredService<IMessenger>().Send<LaunchExecutionProcessStatusChangedMessage>();
    }
}