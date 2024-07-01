// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Database;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Abstraction;

namespace Snap.Hutao.Service.Game;

[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(IGameDbService))]
internal sealed partial class GameDbService : IGameDbService
{
    private readonly IServiceProvider serviceProvider;

    public IServiceProvider ServiceProvider { get => serviceProvider; }

    public ObservableReorderableDbCollection<GameAccount> GetGameAccountCollection()
    {
        return this.Query(query => query.ToObservableReorderableDbCollection(serviceProvider));
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