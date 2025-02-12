// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Game;
using Snap.Hutao.Service.Game.Scheme;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.Service.User;
using Snap.Hutao.UI.Xaml.Data;
using Snap.Hutao.UI.Xaml.View.Page;
using Snap.Hutao.ViewModel.User;

namespace Snap.Hutao.ViewModel.Game;

[Injection(InjectAs.Transient)]
[ConstructorGenerated(CallBaseConstructor = true)]
internal sealed partial class LaunchGameViewModelSlim : Abstraction.ViewModelSlim<LaunchGamePage>, IViewModelSupportLaunchExecution
{
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
}