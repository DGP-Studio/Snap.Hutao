// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Snap.Hutao.Model.Metadata.Avatar;
using Snap.Hutao.Model.Metadata.Item;
using Snap.Hutao.Model.Metadata.Weapon;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.Service.Cultivation;

namespace Snap.Hutao.Service.Metadata.ContextAbstraction;

internal static class MetadataServiceContextExtension
{
    public static async ValueTask<TContext> GetContextAsync<TContext>(this IMetadataService metadataService, CancellationToken token = default)
        where TContext : IMetadataContext, new()
    {
        TContext context = new();

        // List
        {
            if (context is IMetadataListAchievementSource listAchievementSource)
            {
                listAchievementSource.Achievements = await metadataService.GetAchievementListAsync(token).ConfigureAwait(false);
            }

            if (context is IMetadataListGachaEventSource listGachaEventSource)
            {
                listGachaEventSource.GachaEvents = await metadataService.GetGachaEventListAsync(token).ConfigureAwait(false);
            }

            if (context is IMetadataListMaterialSource listMaterialSource)
            {
                listMaterialSource.Materials = await metadataService.GetMaterialListAsync(token).ConfigureAwait(false);
            }
        }

        // Dictionary
        {
            if (context is IMetadataDictionaryIdAchievementSource dictionaryIdAchievementSource)
            {
                dictionaryIdAchievementSource.IdAchievementMap = await metadataService.GetIdToAchievementMapAsync(token).ConfigureAwait(false);
            }

            if (context is IMetadataDictionaryIdAvatarSource dictionaryIdAvatarSource)
            {
                dictionaryIdAvatarSource.IdAvatarMap = await metadataService.GetIdToAvatarMapAsync(token).ConfigureAwait(false);
            }

            if (context is IMetadataDictionaryIdMaterialSource dictionaryIdMaterialSource)
            {
                dictionaryIdMaterialSource.IdMaterialMap = await metadataService.GetIdToMaterialMapAsync(token).ConfigureAwait(false);
            }

            if (context is IMetadataDictionaryIdReliquaryAffixWeightSource dictionaryIdReliquaryAffixWeightSource)
            {
                dictionaryIdReliquaryAffixWeightSource.IdReliquaryAffixWeightMap = await metadataService.GetIdToReliquaryAffixWeightMapAsync(token).ConfigureAwait(false);
            }

            if (context is IMetadataDictionaryIdReliquaryMainPropertySource dictionaryIdReliquaryMainPropertySource)
            {
                dictionaryIdReliquaryMainPropertySource.IdReliquaryMainPropertyMap = await metadataService.GetIdToReliquaryMainPropertyMapAsync(token).ConfigureAwait(false);
            }

            if (context is IMetadataDictionaryIdReliquarySubAffixSource dictionaryIdReliquarySubAffixSource)
            {
                dictionaryIdReliquarySubAffixSource.IdReliquarySubAffixMap = await metadataService.GetIdToReliquarySubAffixMapAsync(token).ConfigureAwait(false);
            }

            if (context is IMetadataDictionaryIdWeaponSource dictionaryIdWeaponSource)
            {
                dictionaryIdWeaponSource.IdWeaponMap = await metadataService.GetIdToWeaponMapAsync(token).ConfigureAwait(false);
            }

            if (context is IMetadataDictionaryNameAvatarSource dictionaryNameAvatarSource)
            {
                dictionaryNameAvatarSource.NameAvatarMap = await metadataService.GetNameToAvatarMapAsync(token).ConfigureAwait(false);
            }

            if (context is IMetadataDictionaryNameWeaponSource dictionaryNameWeaponSource)
            {
                dictionaryNameWeaponSource.NameWeaponMap = await metadataService.GetNameToWeaponMapAsync(token).ConfigureAwait(false);
            }
        }

        if (context is IMetadataSupportInitialization supportInitialization)
        {
            supportInitialization.IsInitialized = true;
        }

        return context;
    }

#pragma warning disable SH002
    public static IEnumerable<Material> EnumerateInventoryMaterial(this IMetadataListMaterialSource context)
    {
        return context.Materials.Where(m => m.IsInventoryItem()).OrderBy(m => m.Id, MaterialIdComparer.Shared);
    }

    public static Avatar GetAvatar(this IMetadataDictionaryIdAvatarSource context, AvatarId id)
    {
        return context.IdAvatarMap[id];
    }

    public static Avatar GetAvatar(this IMetadataDictionaryNameAvatarSource context, string name)
    {
        return context.NameAvatarMap[name];
    }

    public static Material GetMaterial(this IMetadataDictionaryIdMaterialSource context, MaterialId id)
    {
        return context.IdMaterialMap[id];
    }

    public static Weapon GetWeapon(this IMetadataDictionaryIdWeaponSource context, WeaponId id)
    {
        return context.IdWeaponMap[id];
    }

    public static Weapon GetWeapon(this IMetadataDictionaryNameWeaponSource context, string name)
    {
        return context.NameWeaponMap[name];
    }
#pragma warning restore SH002
}