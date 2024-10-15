// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Metadata.Item;
using Snap.Hutao.Service.Cultivation;
using Snap.Hutao.Service.Metadata.ContextAbstraction;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.Service.User;
using Snap.Hutao.ViewModel.Cultivation;
using Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate;
using Snap.Hutao.Web.Response;

namespace Snap.Hutao.Service.Inventory;

[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(IInventoryService))]
internal sealed partial class InventoryService : IInventoryService
{
    private readonly MinimalPromotionDelta minimalPromotionDelta;
    private readonly IServiceScopeFactory serviceScopeFactory;
    private readonly IInventoryRepository inventoryRepository;
    private readonly IInfoBarService infoBarService;
    private readonly IUserService userService;

    /// <inheritdoc/>
    public List<InventoryItemView> GetInventoryItemViews(CultivateProject cultivateProject, ICultivationMetadataContext context, ICommand saveCommand)
    {
        Guid projectId = cultivateProject.InnerId;
        List<InventoryItem> entities = inventoryRepository.GetInventoryItemListByProjectId(projectId);

        List<InventoryItemView> results = [];
        foreach (Material meta in context.EnumerateInventoryMaterial())
        {
            InventoryItem entity = entities.SingleOrDefault(e => e.ItemId == meta.Id) ?? InventoryItem.From(projectId, meta.Id);
            results.Add(new(entity, meta, saveCommand));
        }

        return results;
    }

    /// <inheritdoc/>
    public void SaveInventoryItem(InventoryItemView item)
    {
        inventoryRepository.UpdateInventoryItem(item.Entity);
    }

    /// <inheritdoc/>
    public async ValueTask RefreshInventoryAsync(CultivateProject project)
    {
        List<AvatarPromotionDelta> deltas = await minimalPromotionDelta.GetAsync().ConfigureAwait(false);

        BatchConsumption? batchConsumption;
        using (IServiceScope scope = serviceScopeFactory.CreateScope())
        {
            if (await userService.GetCurrentUserAndUidAsync().ConfigureAwait(false) is not { } userAndUid)
            {
                infoBarService.Warning(SH.MustSelectUserAndUid);
                return;
            }

            CalculateClient calculateClient = scope.ServiceProvider.GetRequiredService<CalculateClient>();

            Response<BatchConsumption> resp = await calculateClient
                .BatchComputeAsync(userAndUid, deltas, true)
                .ConfigureAwait(false);

            if (!resp.IsOk())
            {
                return;
            }

            batchConsumption = resp.Data;
        }

        if (batchConsumption is { OverallConsume: { } items })
        {
            inventoryRepository.RemoveInventoryItemRangeByProjectId(project.InnerId);
            inventoryRepository.AddInventoryItemRangeByProjectId(items.SelectList(item => InventoryItem.From(project.InnerId, item.Id, (uint)((int)item.Num - item.LackNum))));
        }
    }
}