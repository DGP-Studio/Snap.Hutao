// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Entity.Primitive;
using Snap.Hutao.Service.Game.Configuration;
using Snap.Hutao.UI.Xaml.Data;

namespace Snap.Hutao.Service.Game;

internal interface IGameService
{
    ValueTask<GameAccount?> DetectGameAccountAsync(SchemeType scheme);

    ValueTask<ValueResult<bool, string>> GetGamePathAsync();

    ChannelOptions GetChannelOptions();

    bool IsGameRunning();

    bool KillGameProcess();

    ValueTask ModifyGameAccountAsync(GameAccount gameAccount);

    ValueTask RemoveGameAccountAsync(GameAccount gameAccount);

    GameAccount? DetectCurrentGameAccount(SchemeType scheme);

    ValueTask<IAdvancedCollectionView<GameAccount>> GetGameAccountCollectionAsync();
}