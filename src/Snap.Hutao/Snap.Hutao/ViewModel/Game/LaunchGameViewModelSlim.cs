// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Snap.Hutao.Core.Logging;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Game;
using Snap.Hutao.Service.Game.Scheme;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.Service.User;
using Snap.Hutao.UI.Xaml.Data;
using Snap.Hutao.UI.Xaml.View.Page;
using Snap.Hutao.ViewModel.User;
using Snap.Hutao.Web.Hoyolab.Takumi.Binding;

namespace Snap.Hutao.ViewModel.Game;

[Injection(InjectAs.Transient)]
[ConstructorGenerated(CallBaseConstructor = true)]
internal sealed partial class LaunchGameViewModelSlim : Abstraction.ViewModelSlim<LaunchGamePage>,
    IViewModelSupportLaunchExecution,
    IRecipient<UserAndUidChangedMessage>
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

    [ObservableProperty]
    public partial UserGameRole? CurrentUserGameRole { get; set; }

    public void Receive(UserAndUidChangedMessage message)
    {
        if (!LaunchOptions.UsingHoyolabAccount)
        {
            return;
        }

        taskContext.InvokeOnMainThread(() =>
        {
            if (!IsInitialized)
            {
                return;
            }

            CurrentUserGameRole = message.User?.UserGameRoles.CurrentItem;
        });
    }

    protected override async Task LoadAsync()
    {
        Shared.ResumeLaunchExecutionAsync(this).SafeForget();

        UserGameRole? userGameRole = default;
        if (LaunchOptions.UsingHoyolabAccount)
        {
            userGameRole = await userService.GetCurrentUserGameRoleAsync().ConfigureAwait(false);
        }

        LaunchScheme? scheme = Shared.GetCurrentLaunchSchemeFromConfigFile();
        IAdvancedCollectionView<GameAccount> accountsView = await gameService.GetGameAccountCollectionAsync().ConfigureAwait(false);
        accountsView.Filter = GameAccountFilter.CreateFilter(scheme?.GetSchemeType());

        await taskContext.SwitchToMainThreadAsync();
        GameAccountsView = accountsView;
        CurrentUserGameRole = userGameRole;

        try
        {
            if (scheme is null)
            {
                return;
            }

            // Try set to the current account.
            if (GameAccountsView.CurrentItem is null)
            {
                GameAccountsView.MoveCurrentTo(gameService.DetectCurrentGameAccountNoThrow(scheme));
            }
        }
        catch (Exception ex)
        {
            infoBarService.Error(ex);
        }
        finally
        {
            IsInitialized = true;
        }
    }

    [Command("LaunchCommand")]
    private async Task LaunchAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Launch game", "LaunchGameViewModelSlim.Command"));

        UserAndUid? userAndUid = await userService.GetCurrentUserAndUidAsync().ConfigureAwait(false);
        await this.LaunchExecutionAsync(userAndUid).ConfigureAwait(false);
    }
}