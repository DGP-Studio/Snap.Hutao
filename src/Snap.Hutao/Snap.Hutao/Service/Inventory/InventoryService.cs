// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Metadata.Abstraction;
using Snap.Hutao.Model.Metadata.Item;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.Service.Cultivation;
using Snap.Hutao.Service.Metadata;
using Snap.Hutao.Service.Metadata.ContextAbstraction;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.Service.User;
using Snap.Hutao.ViewModel.Cultivation;
using Snap.Hutao.ViewModel.User;
using Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate;
using Snap.Hutao.Web.Response;
using System.Runtime.InteropServices;
using MetadataAvatar = Snap.Hutao.Model.Metadata.Avatar.Avatar;
using MetadataWeapon = Snap.Hutao.Model.Metadata.Weapon.Weapon;

namespace Snap.Hutao.Service.Inventory;

[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(IInventoryService))]
internal sealed partial class InventoryService : IInventoryService
{
    private readonly IServiceScopeFactory serviceScopeFactory;
    private readonly IInventoryDbService inventoryDbService;
    private readonly IMetadataService metadataService;
    private readonly IInfoBarService infoBarService;
    private readonly IUserService userService;

    /// <inheritdoc/>
    public List<InventoryItemView> GetInventoryItemViews(CultivateProject cultivateProject, ICultivationMetadataContext context, ICommand saveCommand)
    {
        Guid projectId = cultivateProject.InnerId;
        List<InventoryItem> entities = inventoryDbService.GetInventoryItemListByProjectId(projectId);

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
        inventoryDbService.UpdateInventoryItem(item.Entity);
    }

    /// <inheritdoc/>
    public async ValueTask RefreshInventoryAsync(CultivateProject project)
    {
        List<ICultivationItemsAccess> cultivationItemsEntryList =
        [
            .. await metadataService.GetAvatarListAsync().ConfigureAwait(false),
            .. await metadataService.GetWeaponListAsync().ConfigureAwait(false),
        ];

        cultivationItemsEntryList = MinimalPromotionDelta.Find(cultivationItemsEntryList);

        BatchConsumption? batchConsumption = default;
        using (IServiceScope scope = serviceScopeFactory.CreateScope())
        {
            if (!UserAndUid.TryFromUser(userService.Current, out UserAndUid? userAndUid))
            {
                infoBarService.Warning(SH.MustSelectUserAndUid);
                return;
            }

            CalculateClient calculateClient = scope.ServiceProvider.GetRequiredService<CalculateClient>();

            Response<BatchConsumption>? resp = await calculateClient
                .BatchComputeAsync(userAndUid, GeneratePromotionDeltas(cultivationItemsEntryList), true)
                .ConfigureAwait(false);

            if (!resp.IsOk())
            {
                return;
            }

            batchConsumption = resp.Data;
        }

        if (batchConsumption is { OverallConsume: { } items })
        {
            await inventoryDbService.RemoveInventoryItemRangeByProjectIdAsync(project.InnerId).ConfigureAwait(false);
            await inventoryDbService.AddInventoryItemRangeByProjectIdAsync(items.SelectList(item => InventoryItem.From(project.InnerId, item.Id, (uint)((int)item.Num - item.LackNum)))).ConfigureAwait(false);
        }
    }

    private static List<AvatarPromotionDelta> GeneratePromotionDeltas(List<ICultivationItemsAccess> cultivatables)
    {
        List<MetadataAvatar> avatars = [];
        List<MetadataWeapon> weapons = [];
        HashSet<MaterialId> materialIds = [];

        while (cultivatables.Count > 0)
        {
            ICultivationItemsAccess bestItem = cultivatables.OrderByDescending(item => item.CultivationItems.Count(material => !materialIds.Contains(material))).First();

            if (bestItem.CultivationItems.All(materialIds.Contains))
            {
                break;
            }

            switch (bestItem)
            {
                case MetadataAvatar avatar:
                    avatars.Add(avatar);
                    break;
                case MetadataWeapon weapon:
                    weapons.Add(weapon);
                    break;
                default:
                    throw HutaoException.NotSupported();
            }

            foreach (ref readonly MaterialId materialId in CollectionsMarshal.AsSpan(bestItem.CultivationItems))
            {
                materialIds.Add(materialId);
            }

            cultivatables.Remove(bestItem);
        }

        List<AvatarPromotionDelta> deltas = [];

        for (int i = 0; i < Math.Max(avatars.Count, weapons.Count); i++)
        {
            MetadataAvatar? avatar = avatars.ElementAtOrDefault(i);
            MetadataWeapon? weapon = weapons.ElementAtOrDefault(i);

            if (avatar is not null)
            {
                AvatarPromotionDelta delta = new()
                {
                    AvatarId = avatar.Id,
                    AvatarLevelCurrent = 1,
                    AvatarLevelTarget = 90,
                    SkillList = avatar.SkillDepot.CompositeSkillsNoInherents().SelectList(skill => new PromotionDelta()
                    {
                        Id = skill.GroupId,
                        LevelCurrent = 1,
                        LevelTarget = 10,
                    }),
                };

                if (weapon is not null)
                {
                    delta.Weapon = new()
                    {
                        Id = weapon.Id,
                        LevelCurrent = 1,
                        LevelTarget = 90,
                    };
                }

                deltas.Add(delta);

                continue;
            }

            if (weapon is not null)
            {
                AvatarPromotionDelta delta = new()
                {
                    Weapon = new()
                    {
                        Id = weapon.Id,
                        LevelCurrent = 1,
                        LevelTarget = 90,
                    },
                };

                deltas.Add(delta);

                continue;
            }
        }

        return deltas;
    }
}