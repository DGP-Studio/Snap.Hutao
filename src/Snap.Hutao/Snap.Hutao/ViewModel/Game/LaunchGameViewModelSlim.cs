// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Game;
using Snap.Hutao.Service.Notification;
using System.Collections.ObjectModel;

namespace Snap.Hutao.ViewModel.Game;

/// <summary>
/// 简化的启动游戏视图模型
/// </summary>
[Injection(InjectAs.Transient)]
[ConstructorGenerated(CallBaseConstructor = true)]
internal sealed partial class LaunchGameViewModelSlim : Abstraction.ViewModelSlim<View.Page.LaunchGamePage>
{
    private readonly IGameServiceFacade gameService;
    private readonly ITaskContext taskContext;
    private readonly IInfoBarService infoBarService;

    private ObservableCollection<GameAccount>? gameAccounts;
    private GameAccount? selectedGameAccount;

    /// <summary>
    /// 游戏账号集合
    /// </summary>
    public ObservableCollection<GameAccount>? GameAccounts { get => gameAccounts; set => SetProperty(ref gameAccounts, value); }

    /// <summary>
    /// 选中的账号
    /// </summary>
    public GameAccount? SelectedGameAccount { get => selectedGameAccount; set => SetProperty(ref selectedGameAccount, value); }

    /// <inheritdoc/>
    protected override async Task OpenUIAsync()
    {
        ObservableCollection<GameAccount> accounts = gameService.GameAccountCollection;
        await taskContext.SwitchToMainThreadAsync();
        GameAccounts = accounts;

        try
        {
            // Try set to the current account.
            SelectedGameAccount ??= gameService.DetectCurrentGameAccount();
        }
        catch (UserdataCorruptedException ex)
        {
            infoBarService.Error(ex);
        }
    }

    [Command("LaunchCommand", AllowConcurrentExecutions = true)]
    private async Task LaunchAsync()
    {
        IInfoBarService infoBarService = ServiceProvider.GetRequiredService<IInfoBarService>();

        try
        {
            if (SelectedGameAccount is not null)
            {
                if (!gameService.SetGameAccount(SelectedGameAccount))
                {
                    infoBarService.Warning(SH.ViewModelLaunchGameSwitchGameAccountFail);
                    return;
                }
            }

            await gameService.LaunchAsync(new Progress<LaunchStatus>()).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            infoBarService.Error(ex);
        }
    }
}