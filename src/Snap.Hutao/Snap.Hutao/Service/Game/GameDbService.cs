// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using Snap.Hutao.Core.Database;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Entity.Database;

namespace Snap.Hutao.Service.Game;

[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(IGameDbService))]
internal sealed partial class GameDbService : IGameDbService
{
    private readonly IServiceProvider serviceProvider;

    public ObservableReorderableDbCollection<GameAccount> GetGameAccountCollection()
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            return appDbContext.GameAccounts.AsNoTracking().ToObservableReorderableDbCollection(serviceProvider);
        }
    }

    public async ValueTask AddGameAccountAsync(GameAccount gameAccount)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await appDbContext.GameAccounts.AddAndSaveAsync(gameAccount).ConfigureAwait(false);
        }
    }

    public void UpdateGameAccount(GameAccount gameAccount)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            appDbContext.GameAccounts.UpdateAndSave(gameAccount);
        }
    }

    public async ValueTask UpdateGameAccountAsync(GameAccount gameAccount)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await appDbContext.GameAccounts.UpdateAndSaveAsync(gameAccount).ConfigureAwait(false);
        }
    }

    public async ValueTask RemoveGameAccountByIdAsync(Guid id)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await appDbContext.GameAccounts.ExecuteDeleteWhereAsync(a => a.InnerId == id).ConfigureAwait(false);
        }
    }
}