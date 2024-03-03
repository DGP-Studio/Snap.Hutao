// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Metadata.Avatar;
using Snap.Hutao.Model.Metadata.Item;
using Snap.Hutao.Model.Metadata.Weapon;
using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Service.Metadata.ContextAbstraction;

internal static class MetadataServiceContextExtension
{
    public static async ValueTask<TContext> GetContextAsync<TContext>(this IMetadataService metadataService, CancellationToken token = default)
        where TContext : IMetadataContext, new()
    {
        TContext context = new();

        // List
        {
            if (context is IMetadataListMaterialSource listMaterialSource)
            {
                listMaterialSource.Materials = await metadataService.GetMaterialListAsync(token).ConfigureAwait(false);
            }

            if (context is IMetadataListGachaEventSource listGachaEventSource)
            {
                listGachaEventSource.GachaEvents = await metadataService.GetGachaEventListAsync(token).ConfigureAwait(false);
            }
        }

        // Dictionary
        {
            if (context is IMetadataDictionaryIdAvatarSource dictionaryAvatarSource)
            {
                dictionaryAvatarSource.IdAvatarMap = await metadataService.GetIdToAvatarMapAsync(token).ConfigureAwait(false);
            }

            if (context is IMetadataDictionaryIdMaterialSource dictionaryMaterialSource)
            {
                dictionaryMaterialSource.IdMaterialMap = await metadataService.GetIdToMaterialMapAsync(token).ConfigureAwait(false);
            }

            if (context is IMetadataDictionaryIdWeaponSource dictionaryWeaponSource)
            {
                dictionaryWeaponSource.IdWeaponMap = await metadataService.GetIdToWeaponMapAsync(token).ConfigureAwait(false);
            }
        }

        return context;
    }

#pragma warning disable SH002
    public static IEnumerable<Material> EnumerateInventroyMaterial(this IMetadataListMaterialSource context)
    {
        return context.Materials.Where(m => m.IsInventoryItem()).OrderBy(m => m.Id.Value);
    }

    public static Avatar GetAvatar(this IMetadataDictionaryIdAvatarSource context, AvatarId id)
    {
        return context.IdAvatarMap[id];
    }

    public static Material GetMaterial(this IMetadataDictionaryIdMaterialSource context, MaterialId id)
    {
        return context.IdMaterialMap[id];
    }

    public static Weapon GetWeapon(this IMetadataDictionaryIdWeaponSource context, WeaponId id)
    {
        return context.IdWeaponMap[id];
    }
#pragma warning restore SH002
}