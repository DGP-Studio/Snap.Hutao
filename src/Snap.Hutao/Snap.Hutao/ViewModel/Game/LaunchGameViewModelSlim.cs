// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Game;
using Snap.Hutao.Service.Game.Scheme;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.UI.Xaml.Data;
using System.Collections.ObjectModel;

namespace Snap.Hutao.ViewModel.Game;

/// <summary>
/// 简化的启动游戏视图模型
/// </summary>
[Injection(InjectAs.Transient)]
[ConstructorGenerated(CallBaseConstructor = true)]
internal sealed partial class LaunchGameViewModelSlim : Abstraction.ViewModelSlim<View.Page.LaunchGamePage>, IViewModelSupportLaunchExecution
{
    private readonly LaunchStatusOptions launchStatusOptions;
    private readonly LaunchGameShared launchGameShared;
    private readonly IInfoBarService infoBarService;
    private readonly IGameServiceFacade gameService;
    private readonly ITaskContext taskContext;

    private AdvancedCollectionView<GameAccount>? gameAccountsView;
    private GameAccount? selectedGameAccount;
    private GameAccountFilter? gameAccountFilter;

    LaunchGameShared IViewModelSupportLaunchExecution.Shared { get => launchGameShared; }

    public LaunchStatusOptions LaunchStatusOptions { get => launchStatusOptions; }

    public AdvancedCollectionView<GameAccount>? GameAccountsView { get => gameAccountsView; set => SetProperty(ref gameAccountsView, value); }

    /// <summary>
    /// 选中的账号
    /// </summary>
    public GameAccount? SelectedGameAccount { get => selectedGameAccount; set => SetProperty(ref selectedGameAccount, value); }

    /// <inheritdoc/>
    protected override async Task OpenUIAsync()
    {
        LaunchScheme? scheme = launchGameShared.GetCurrentLaunchSchemeFromConfigFile();
        ObservableCollection<GameAccount> accounts = gameService.GameAccountCollection;

        try
        {
            if (scheme is not null)
            {
                // Try set to the current account.
                SelectedGameAccount ??= gameService.DetectCurrentGameAccount(scheme);
            }
        }
        catch (Exception ex)
        {
            infoBarService.Error(ex);
        }

        gameAccountFilter = new(scheme?.GetSchemeType());

        await taskContext.SwitchToMainThreadAsync();
        GameAccountsView = new(accounts, true)
        {
            Filter = gameAccountFilter.Filter,
        };
    }

    [Command("LaunchCommand")]
    private async Task LaunchAsync()
    {
        await this.LaunchExecutionAsync(launchGameShared.GetCurrentLaunchSchemeFromConfigFile()).ConfigureAwait(false);
    }
}