// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Entity.Primitive;
using Snap.Hutao.Service.Game.Account;
using Snap.Hutao.Service.Game.Configuration;
using Snap.Hutao.Service.Game.Launching.Handler;
using Snap.Hutao.Service.Game.PathAbstraction;
using System.Collections.ObjectModel;

namespace Snap.Hutao.Service.Game;

/// <summary>
/// 游戏服务
/// </summary>
[HighQuality]
[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(IGameServiceFacade))]
internal sealed partial class GameServiceFacade : IGameServiceFacade
{
    private readonly IGameChannelOptionsService gameChannelOptionsService;
    private readonly IGameAccountService gameAccountService;
    private readonly IGamePathService gamePathService;

    /// <inheritdoc/>
    public ObservableCollection<GameAccount> GameAccountCollection
    {
        get => gameAccountService.GameAccountCollection;
    }

    /// <inheritdoc/>
    public ValueTask<ValueResult<bool, string>> GetGamePathAsync()
    {
        return gamePathService.SilentGetGamePathAsync();
    }

    /// <inheritdoc/>
    public ChannelOptions GetChannelOptions()
    {
        return gameChannelOptionsService.GetChannelOptions();
    }

    /// <inheritdoc/>
    public ValueTask<GameAccount?> DetectGameAccountAsync(SchemeType scheme)
    {
        return gameAccountService.DetectGameAccountAsync(scheme);
    }

    /// <inheritdoc/>
    public GameAccount? DetectCurrentGameAccount(SchemeType scheme)
    {
        return gameAccountService.DetectCurrentGameAccount(scheme);
    }

    /// <inheritdoc/>
    public void AttachGameAccountToUid(GameAccount gameAccount, string uid)
    {
        gameAccountService.AttachGameAccountToUid(gameAccount, uid);
    }

    /// <inheritdoc/>
    public ValueTask ModifyGameAccountAsync(GameAccount gameAccount)
    {
        return gameAccountService.ModifyGameAccountAsync(gameAccount);
    }

    /// <inheritdoc/>
    public ValueTask RemoveGameAccountAsync(GameAccount gameAccount)
    {
        return gameAccountService.RemoveGameAccountAsync(gameAccount);
    }

    /// <inheritdoc/>
    public bool IsGameRunning()
    {
        return LaunchExecutionEnsureGameNotRunningHandler.IsGameRunning(out _);
    }

    public void ReorderGameAccounts(IEnumerable<GameAccount> reorderedGameAccounts)
    {
        gameAccountService.ReorderGameAccounts(reorderedGameAccounts);
    }
}