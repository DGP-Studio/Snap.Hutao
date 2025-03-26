// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Entity.Primitive;
using Snap.Hutao.Service.Game.Account;
using Snap.Hutao.Service.Game.Configuration;
using Snap.Hutao.Service.Game.Launching.Handler;
using Snap.Hutao.Service.Game.PathAbstraction;
using Snap.Hutao.UI.Xaml.Data;

namespace Snap.Hutao.Service.Game;

[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(IGameService))]
internal sealed partial class GameService : IGameService
{
    private readonly IGameChannelOptionsService gameChannelOptionsService;
    private readonly IGameAccountService gameAccountService;
    private readonly IGamePathService gamePathService;

    public ValueTask<IAdvancedCollectionView<GameAccount>> GetGameAccountCollectionAsync()
    {
        return gameAccountService.GetGameAccountCollectionAsync();
    }

    public ValueTask<ValueResult<bool, string>> GetGamePathAsync()
    {
        return gamePathService.SilentGetGamePathAsync();
    }

    public ChannelOptions GetChannelOptions()
    {
        return gameChannelOptionsService.GetChannelOptions();
    }

    public ValueTask<GameAccount?> DetectGameAccountAsync(SchemeType scheme)
    {
        return gameAccountService.DetectCurrentGameAccountAsync(scheme);
    }

    public GameAccount? DetectCurrentGameAccount(SchemeType scheme)
    {
        return gameAccountService.DetectCurrentGameAccount(scheme);
    }

    public ValueTask ModifyGameAccountAsync(GameAccount gameAccount)
    {
        return gameAccountService.ModifyGameAccountAsync(gameAccount);
    }

    public ValueTask RemoveGameAccountAsync(GameAccount gameAccount)
    {
        return gameAccountService.RemoveGameAccountAsync(gameAccount);
    }

    public bool IsGameRunning()
    {
        return LaunchExecutionEnsureGameNotRunningHandler.IsGameRunning();
    }
}