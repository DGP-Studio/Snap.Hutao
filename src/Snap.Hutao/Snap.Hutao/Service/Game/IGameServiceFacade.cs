// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Database;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Entity.Primitive;
using Snap.Hutao.Service.Game.Configuration;

namespace Snap.Hutao.Service.Game;

internal interface IGameServiceFacade
{
    ObservableReorderableDbCollection<GameAccount> GameAccountCollection { get; }

    ValueTask AttachGameAccountToUidAsync(GameAccount gameAccount, string uid);

    ValueTask<GameAccount?> DetectGameAccountAsync(SchemeType scheme);

    ValueTask<ValueResult<bool, string>> GetGamePathAsync();

    ChannelOptions GetChannelOptions();

    bool IsGameRunning();

    ValueTask ModifyGameAccountAsync(GameAccount gameAccount);

    ValueTask RemoveGameAccountAsync(GameAccount gameAccount);

    GameAccount? DetectCurrentGameAccount(SchemeType scheme);
}