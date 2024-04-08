// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Control.Collection.AdvancedCollectionView;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Game;
using Snap.Hutao.Service.Game.Launching;
using Snap.Hutao.Service.Game.PathAbstraction;
using Snap.Hutao.Service.Game.Scheme;
using Snap.Hutao.Service.Notification;
using System.Collections.Immutable;
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
    private readonly ILogger<LaunchGameViewModelSlim> logger;
    private readonly LaunchGameShared launchGameShared;
    private readonly IInfoBarService infoBarService;
    private readonly IGameServiceFacade gameService;
    private readonly ITaskContext taskContext;

    private AdvancedCollectionView<GameAccount>? gameAccountsView;
    private GameAccount? selectedGameAccount;
    private GameAccountFilter? gameAccountFilter;

    public LaunchStatusOptions LaunchStatusOptions { get => launchStatusOptions; }

    public AdvancedCollectionView<GameAccount>? GameAccountsView { get => gameAccountsView; set => SetProperty(ref gameAccountsView, value); }

    /// <summary>
    /// 选中的账号
    /// </summary>
    public GameAccount? SelectedGameAccount { get => selectedGameAccount; set => SetProperty(ref selectedGameAccount, value); }

    public void SetGamePathEntriesAndSelectedGamePathEntry(ImmutableList<GamePathEntry> gamePathEntries, GamePathEntry? selectedEntry)
    {
    }

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
        LaunchScheme? scheme = launchGameShared.GetCurrentLaunchSchemeFromConfigFile();

        try
        {
            LaunchExecutionContext context = new(Ioc.Default, this, scheme, SelectedGameAccount);
            LaunchExecutionResult result = await new LaunchExecutionInvoker().InvokeAsync(context).ConfigureAwait(false);

            if (result.Kind is not LaunchExecutionResultKind.Ok)
            {
                infoBarService.Warning(result.ErrorMessage);
            }
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Launch failed");
            infoBarService.Error(ex);
        }
    }
}