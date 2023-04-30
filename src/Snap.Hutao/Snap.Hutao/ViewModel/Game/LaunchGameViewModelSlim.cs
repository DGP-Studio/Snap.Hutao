// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Input;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Abstraction;
using Snap.Hutao.Service.Game;
using System.Collections.ObjectModel;

namespace Snap.Hutao.ViewModel.Game;

/// <summary>
/// 简化的启动游戏视图模型
/// </summary>
[Injection(InjectAs.Transient)]
internal sealed class LaunchGameViewModelSlim : Abstraction.ViewModelSlim<View.Page.LaunchGamePage>
{
    private readonly IGameService gameService;
    private readonly ITaskContext taskContext;

    private ObservableCollection<GameAccount>? gameAccounts;
    private GameAccount? selectedGameAccount;

    /// <summary>
    /// 构造一个新的简化的启动游戏视图模型
    /// </summary>
    /// <param name="serviceProvider">服务提供器</param>
    public LaunchGameViewModelSlim(IServiceProvider serviceProvider)
        : base(serviceProvider)
    {
        gameService = serviceProvider.GetRequiredService<IGameService>();
        taskContext = serviceProvider.GetRequiredService<ITaskContext>();

        LaunchCommand = new AsyncRelayCommand(LaunchAsync, AsyncRelayCommandOptions.AllowConcurrentExecutions);
    }

    /// <summary>
    /// 游戏账号集合
    /// </summary>
    public ObservableCollection<GameAccount>? GameAccounts { get => gameAccounts; set => SetProperty(ref gameAccounts, value); }

    /// <summary>
    /// 选中的账号
    /// </summary>
    public GameAccount? SelectedGameAccount { get => selectedGameAccount; set => SetProperty(ref selectedGameAccount, value); }

    /// <summary>
    /// 启动游戏命令
    /// </summary>
    public ICommand LaunchCommand { get; }

    /// <inheritdoc/>
    protected override async Task OpenUIAsync()
    {
        ObservableCollection<GameAccount> accounts = gameService.GameAccountCollection;
        await taskContext.SwitchToMainThreadAsync();
        GameAccounts = accounts;

        // Try set to the current account.
        SelectedGameAccount ??= gameService.DetectCurrentGameAccount();
    }

    private async Task LaunchAsync()
    {
        IInfoBarService infoBarService = ServiceProvider.GetRequiredService<IInfoBarService>();

        try
        {
            if (SelectedGameAccount != null)
            {
                if (!gameService.SetGameAccount(SelectedGameAccount))
                {
                    infoBarService.Warning(SH.ViewModelLaunchGameSwitchGameAccountFail);
                    return;
                }
            }

            await gameService.LaunchAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            infoBarService.Error(ex);
        }
    }
}