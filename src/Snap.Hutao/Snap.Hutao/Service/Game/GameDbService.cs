// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using Snap.Hutao.Core.Database;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Entity.Database;
using System.Collections.ObjectModel;

namespace Snap.Hutao.Service.Game;

[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(IGameDbService))]
internal sealed partial class GameDbService : IGameDbService
{
    private readonly IServiceProvider serviceProvider;

    public ObservableCollection<GameAccount> GetGameAccountCollection()
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            return appDbContext.GameAccounts.AsNoTracking().ToObservableCollection();
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

    public void ReorderGameAccounts(IEnumerable<GameAccount> reorderedGameAccounts)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            appDbContext.GameAccounts.ExecuteDelete();

            // Use foreach to avoid ORDER BY InnerId
            foreach (GameAccount gameAccount in reorderedGameAccounts)
            {
                appDbContext.GameAccounts.AddAndSave(gameAccount);
            }
        }
    }
}