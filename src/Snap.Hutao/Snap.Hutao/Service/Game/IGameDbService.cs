// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Database;
using Snap.Hutao.Model.Entity;

namespace Snap.Hutao.Service.Game;

internal interface IGameDbService
{
    ValueTask AddGameAccountAsync(GameAccount gameAccount);

    ValueTask RemoveGameAccountByIdAsync(Guid id);

    ObservableReorderableDbCollection<GameAccount> GetGameAccountCollection();

    void UpdateGameAccount(GameAccount gameAccount);

    ValueTask UpdateGameAccountAsync(GameAccount gameAccount);
}