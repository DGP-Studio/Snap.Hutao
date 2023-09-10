// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;
using System.Collections.ObjectModel;

namespace Snap.Hutao.Service.Game;

internal interface IGameDbService
{
    ValueTask AddGameAccountAsync(GameAccount gameAccount);

    ValueTask RemoveGameAccountByIdAsync(Guid id);

    ObservableCollection<GameAccount> GetGameAccountCollection();

    void UpdateGameAccount(GameAccount gameAccount);

    ValueTask UpdateGameAccountAsync(GameAccount gameAccount);
}