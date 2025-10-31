// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Entity.Primitive;
using Snap.Hutao.Service.Game.Account;
using Snap.Hutao.Service.Game.Configuration;
using Snap.Hutao.Service.Game.PathAbstraction;
using Snap.Hutao.UI.Xaml.Data;

namespace Snap.Hutao.Service.Game;

[Service(ServiceLifetime.Singleton, typeof(IGameService))]
internal sealed partial class GameService : IGameService
{
    private readonly IGameInRegistryAccountService gameInRegistryAccountService;
    private readonly IGameChannelOptionsService gameChannelOptionsService;
    private readonly IGamePathService gamePathService;

    [GeneratedConstructor]
    public partial GameService(IServiceProvider serviceProvider);

    public ValueTask<IAdvancedCollectionView<GameAccount>> GetGameAccountCollectionAsync()
    {
        return gameInRegistryAccountService.GetGameAccountCollectionAsync();
    }

    public ValueTask<ValueResult<bool, string>> GetGamePathAsync()
    {
        return gamePathService.SilentLocateGamePathAsync();
    }

    public ChannelOptions GetChannelOptions()
    {
        return gameChannelOptionsService.GetChannelOptions();
    }

    public ValueTask<GameAccount?> DetectGameAccountAsync(SchemeType scheme, Func<string, Task<ValueResult<bool, string?>>> providerNameCallback)
    {
        return gameInRegistryAccountService.DetectCurrentGameAccountAsync(scheme, providerNameCallback);
    }

    public GameAccount? DetectCurrentGameAccount(SchemeType scheme)
    {
        return gameInRegistryAccountService.DetectCurrentGameAccount(scheme);
    }

    public ValueTask ModifyGameAccountAsync(GameAccount gameAccount, Func<string, Task<ValueResult<bool, string?>>> providerNameCallback)
    {
        return gameInRegistryAccountService.ModifyGameAccountAsync(gameAccount, providerNameCallback);
    }

    public ValueTask RemoveGameAccountAsync(GameAccount gameAccount)
    {
        return gameInRegistryAccountService.RemoveGameAccountAsync(gameAccount);
    }
}