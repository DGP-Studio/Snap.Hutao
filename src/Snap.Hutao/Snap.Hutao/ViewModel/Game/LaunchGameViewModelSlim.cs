// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Messaging;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Game;
using Snap.Hutao.Service.Game.Scheme;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.Service.User;
using Snap.Hutao.UI.Xaml.Data;
using Snap.Hutao.UI.Xaml.View.Page;
using Snap.Hutao.ViewModel.User;
using System.Collections.ObjectModel;

namespace Snap.Hutao.ViewModel.Game;

/// <summary>
/// 简化的启动游戏视图模型
/// </summary>
[Injection(InjectAs.Transient)]
[ConstructorGenerated(CallBaseConstructor = true)]
internal sealed partial class LaunchGameViewModelSlim : Abstraction.ViewModelSlim<LaunchGamePage>, IViewModelSupportLaunchExecution, IRecipient<UserAndUidChangedMessage>
{
    private readonly LaunchStatusOptions launchStatusOptions;
    private readonly LaunchGameShared launchGameShared;
    private readonly IInfoBarService infoBarService;
    private readonly IGameServiceFacade gameService;
    private readonly IUserService userService;
    private readonly ITaskContext taskContext;

    private AdvancedCollectionView<GameAccount>? gameAccountsView;
    private GameAccount? selectedGameAccount;
    private UserAndUid? selectedUserAndUid;
    private GameAccountFilter? gameAccountFilter;

    LaunchGameShared IViewModelSupportLaunchExecution.Shared { get => launchGameShared; }

    public LaunchStatusOptions LaunchStatusOptions { get => launchStatusOptions; }

    public AdvancedCollectionView<GameAccount>? GameAccountsView { get => gameAccountsView; set => SetProperty(ref gameAccountsView, value); }

    /// <summary>
    /// 选中的账号
    /// </summary>
    public GameAccount? SelectedGameAccount { get => selectedGameAccount; set => SetProperty(ref selectedGameAccount, value); }

    public UserAndUid? SelectedUserAndUid { get => selectedUserAndUid; set => SetProperty(ref selectedUserAndUid, value); }

    public void Receive(UserAndUidChangedMessage message)
    {
        if (message.UserAndUid is { } userAndUid)
        {
            SelectedUserAndUid = userAndUid;
        }
    }

    /// <inheritdoc/>
    protected override async Task LoadAsync()
    {
        LaunchScheme? scheme = launchGameShared.GetCurrentLaunchSchemeFromConfigFile();
        ObservableCollection<GameAccount> accounts = await gameService.GetGameAccountCollectionAsync().ConfigureAwait(false);

        try
        {
            if (scheme is not null)
            {
                // Try set to the current account.
                await taskContext.SwitchToMainThreadAsync();
                SelectedGameAccount ??= gameService.DetectCurrentGameAccount(scheme);
            }
        }
        catch (Exception ex)
        {
            infoBarService.Error(ex);
        }

        if (await userService.GetCurrentUserAndUidAsync().ConfigureAwait(true) is { } userAndUid)
        {
            SelectedUserAndUid = userAndUid;
        }

        gameAccountFilter = new(scheme?.GetSchemeType());
        AdvancedCollectionView<GameAccount> accountsView = new(accounts) { Filter = gameAccountFilter.Filter };

        await taskContext.SwitchToMainThreadAsync();
        GameAccountsView = accountsView;
    }

    [Command("LaunchCommand")]
    private async Task LaunchAsync()
    {
        await this.LaunchExecutionAsync(launchGameShared.GetCurrentLaunchSchemeFromConfigFile()).ConfigureAwait(false);
    }
}