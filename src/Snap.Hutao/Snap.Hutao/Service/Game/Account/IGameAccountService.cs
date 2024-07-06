// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Database;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Entity.Primitive;

namespace Snap.Hutao.Service.Game.Account;

internal interface IGameAccountService
{
    ObservableReorderableDbCollection<GameAccount> GameAccountCollection { get; }

    ValueTask AttachGameAccountToUidAsync(GameAccount gameAccount, string uid);

    GameAccount? DetectCurrentGameAccount(SchemeType schemeType);

    ValueTask<GameAccount?> DetectGameAccountAsync(SchemeType schemeType);

    ValueTask ModifyGameAccountAsync(GameAccount gameAccount);

    ValueTask RemoveGameAccountAsync(GameAccount gameAccount);

    bool SetGameAccount(GameAccount account);
}