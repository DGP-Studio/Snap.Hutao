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
using System.Collections.Immutable;

namespace Snap.Hutao.Service.Inventory;

[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(IInventoryService))]
internal sealed partial class InventoryService : IInventoryService
{
    private readonly PromotionDeltaFactory promotionDeltaFactory;
    private readonly IServiceScopeFactory serviceScopeFactory;
    private readonly IInventoryRepository inventoryRepository;
    private readonly IInfoBarService infoBarService;
    private readonly IUserService userService;

    public ImmutableArray<InventoryItemView> GetInventoryItemViews(ICultivationMetadataContext context, CultivateProject cultivateProject, ICommand saveCommand)
    {
        Guid projectId = cultivateProject.InnerId;
        ImmutableArray<InventoryItem> entities = inventoryRepository.GetInventoryItemImmutableArrayByProjectId(projectId);

        ImmutableArray<InventoryItemView>.Builder results = ImmutableArray.CreateBuilder<InventoryItemView>();
        foreach (Material meta in context.EnumerateInventoryMaterial())
        {
            InventoryItem entity = entities.SingleOrDefault(e => e.ItemId == meta.Id) ?? InventoryItem.From(projectId, meta.Id);
            results.Add(new(entity, meta, saveCommand));
        }

        return results.ToImmutable();
    }

    public void SaveInventoryItem(InventoryItemView item)
    {
        inventoryRepository.UpdateInventoryItem(item.Entity);
    }

    public async ValueTask RefreshInventoryAsync(ICultivationMetadataContext context, CultivateProject project)
    {
        if (await userService.GetCurrentUserAndUidAsync().ConfigureAwait(false) is not { } userAndUid)
        {
            infoBarService.Warning(SH.MustSelectUserAndUid);
            return;
        }

        ImmutableArray<AvatarPromotionDelta> deltas = await promotionDeltaFactory.GetAsync(context, userAndUid).ConfigureAwait(false);

        BatchConsumption? batchConsumption;
        using (IServiceScope scope = serviceScopeFactory.CreateScope())
        {
            CalculateClient calculateClient = scope.ServiceProvider.GetRequiredService<CalculateClient>();

            Response<BatchConsumption> resp = await calculateClient
                .BatchComputeAsync(userAndUid, deltas, true)
                .ConfigureAwait(false);

            if (!ResponseValidator.TryValidate(resp, scope.ServiceProvider, out batchConsumption))
            {
                return;
            }
        }

        if (batchConsumption is { OverallConsume: { } items })
        {
            inventoryRepository.RemoveInventoryItemRangeByProjectId(project.InnerId);
            inventoryRepository.AddInventoryItemRangeByProjectId(items.SelectList(item => InventoryItem.From(project.InnerId, item.Id, (uint)((int)item.Num - item.LackNum))));
        }
    }
}