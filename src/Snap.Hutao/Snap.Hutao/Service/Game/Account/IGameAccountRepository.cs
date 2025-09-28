// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Database;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Abstraction;

namespace Snap.Hutao.Service.Game.Account;

internal interface IGameAccountRepository : IRepository<GameAccount>
{
    void AddGameAccount(GameAccount gameAccount);

    void RemoveGameAccountById(Guid id);

    ObservableReorderableDbCollection<GameAccount> GetGameAccountCollection();

    void UpdateGameAccount(GameAccount gameAccount);
}