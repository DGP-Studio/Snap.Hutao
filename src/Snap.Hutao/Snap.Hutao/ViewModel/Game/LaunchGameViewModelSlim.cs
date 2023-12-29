// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI.Collections;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Factory.Progress;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Game;
using Snap.Hutao.Service.Game.Scheme;
using Snap.Hutao.Service.Notification;
using System.Collections.ObjectModel;
using Windows.Win32.Foundation;

namespace Snap.Hutao.ViewModel.Game;

/// <summary>
/// 简化的启动游戏视图模型
/// </summary>
[Injection(InjectAs.Transient)]
[ConstructorGenerated(CallBaseConstructor = true)]
internal sealed partial class LaunchGameViewModelSlim : Abstraction.ViewModelSlim<View.Page.LaunchGamePage>
{
    private readonly LaunchStatusOptions launchStatusOptions;
    private readonly IProgressFactory progressFactory;
    private readonly IInfoBarService infoBarService;
    private readonly IGameServiceFacade gameService;
    private readonly ITaskContext taskContext;

    private AdvancedCollectionView? gameAccountsView;
    private GameAccount? selectedGameAccount;
    private GameAccountFilter? gameAccountFilter;

    public AdvancedCollectionView? GameAccountsView { get => gameAccountsView; set => SetProperty(ref gameAccountsView, value); }

    /// <summary>
    /// 选中的账号
    /// </summary>
    public GameAccount? SelectedGameAccount { get => selectedGameAccount; set => SetProperty(ref selectedGameAccount, value); }

    /// <inheritdoc/>
    protected override async Task OpenUIAsync()
    {
        LaunchScheme? scheme = LaunchGameShared.GetCurrentLaunchSchemeFromConfigFile(gameService, infoBarService);
        ObservableCollection<GameAccount> accounts = gameService.GameAccountCollection;

        try
        {
            if (scheme is not null)
            {
                // Try set to the current account.
                SelectedGameAccount ??= gameService.DetectCurrentGameAccount(scheme);
            }
        }
        catch (UserdataCorruptedException ex)
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

            IProgress<LaunchStatus> launchProgress = progressFactory.CreateForMainThread<LaunchStatus>(status => launchStatusOptions.LaunchStatus = status);
            await gameService.LaunchAsync(launchProgress).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            if (ex is Win32Exception win32Exception && win32Exception.HResult == HRESULT.E_FAIL)
            {
                // User canceled the operation. ignore
                return;
            }

            infoBarService.Error(ex);
        }
    }
}