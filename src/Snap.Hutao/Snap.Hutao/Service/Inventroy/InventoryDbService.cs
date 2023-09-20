// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using Snap.Hutao.Core.Database;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Entity.Database;

namespace Snap.Hutao.Service.Inventroy;

[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(IInventoryDbService))]
internal sealed partial class InventoryDbService : IInventoryDbService
{
    private readonly IServiceProvider serviceProvider;

    public async ValueTask RemoveInventoryItemRangeByProjectId(Guid projectId)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await appDbContext.InventoryItems
                .AsNoTracking()
                .Where(a => a.ProjectId == projectId && a.ItemId != 202U) // 摩拉
                .ExecuteDeleteAsync()
                .ConfigureAwait(false);
        }
    }

    public async ValueTask AddInventoryItemRangeByProjectId(List<InventoryItem> items)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await appDbContext.InventoryItems.AddRangeAndSaveAsync(items).ConfigureAwait(false);
        }
    }

    public void UpdateInventoryItem(InventoryItem item)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            appDbContext.InventoryItems.UpdateAndSave(item);
        }
    }

    public async ValueTask UpdateInventoryItemAsync(InventoryItem item)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await appDbContext.InventoryItems.UpdateAndSaveAsync(item).ConfigureAwait(false);
        }
    }
}
