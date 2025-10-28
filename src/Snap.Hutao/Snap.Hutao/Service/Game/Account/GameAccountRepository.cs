// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Database;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Abstraction;

namespace Snap.Hutao.Service.Game.Account;

[ConstructorGenerated]
[Service(ServiceLifetime.Singleton, typeof(IGameAccountRepository))]
internal sealed partial class GameAccountRepository : IGameAccountRepository
{
    public partial IServiceProvider ServiceProvider { get; }

    public ObservableReorderableDbCollection<GameAccount> GetGameAccountCollection()
    {
        return this.Query(query => query.ToObservableReorderableDbCollection(ServiceProvider));
    }

    public void AddGameAccount(GameAccount gameAccount)
    {
        this.Add(gameAccount);
    }

    public void UpdateGameAccount(GameAccount gameAccount)
    {
        this.Update(gameAccount);
    }

    public void RemoveGameAccountById(Guid id)
    {
        this.DeleteByInnerId(id);
    }
}