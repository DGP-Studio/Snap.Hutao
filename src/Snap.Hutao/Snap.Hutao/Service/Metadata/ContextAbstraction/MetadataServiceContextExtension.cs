// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Metadata.Avatar;
using Snap.Hutao.Model.Metadata.Item;
using Snap.Hutao.Model.Metadata.Weapon;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.Service.Cultivation;

namespace Snap.Hutao.Service.Metadata.ContextAbstraction;

internal static class MetadataServiceContextExtension
{
    [SuppressMessage("", "CA1502")]
    [SuppressMessage("", "CA1506")]
    public static async ValueTask<TContext> GetContextAsync<TContext>(this IMetadataService metadataService, CancellationToken token = default)
        where TContext : IMetadataContext, new()
    {
        TContext context = new();

        // Array
        {
            if (context is IMetadataArrayAchievementSource arrayAchievementSource)
            {
                arrayAchievementSource.Achievements = await metadataService.GetAchievementArrayAsync(token).ConfigureAwait(false);
            }

            if (context is IMetadataArrayAvatarSource arrayAvatarSource)
            {
                arrayAvatarSource.Avatars = await metadataService.GetAvatarArrayAsync(token).ConfigureAwait(false);
            }

            if (context is IMetadataArrayChapterSource arrayChapterSource)
            {
                arrayChapterSource.Chapters = await metadataService.GetChapterArrayAsync(token).ConfigureAwait(false);
            }

            if (context is IMetadataArrayGachaEventSource arrayGachaEventSource)
            {
                arrayGachaEventSource.GachaEvents = await metadataService.GetGachaEventArrayAsync(token).ConfigureAwait(false);
            }

            if (context is IMetadataArrayMaterialSource arrayMaterialSource)
            {
                arrayMaterialSource.Materials = await metadataService.GetMaterialArrayAsync(token).ConfigureAwait(false);
            }

            if (context is IMetadataArrayMonsterSource arrayMonsterSource)
            {
                arrayMonsterSource.Monsters = await metadataService.GetMonsterArrayAsync(token).ConfigureAwait(false);
            }

            if (context is IMetadataArrayProfilePictureSource arrayProfilePictureSource)
            {
                arrayProfilePictureSource.ProfilePictures = await metadataService.GetProfilePictureArrayAsync(token).ConfigureAwait(false);
            }

            if (context is IMetadataArrayReliquaryMainAffixLevelSource arrayReliquaryMainAffixLevelSource)
            {
                arrayReliquaryMainAffixLevelSource.ReliquaryMainAffixLevels = await metadataService.GetReliquaryMainAffixLevelArrayAsync(token).ConfigureAwait(false);
            }

            if (context is IMetadataArrayReliquarySource arrayReliquarySource)
            {
                arrayReliquarySource.Reliquaries = await metadataService.GetReliquaryArrayAsync(token).ConfigureAwait(false);
            }

            if (context is IMetadataArrayWeaponSource arrayWeaponSource)
            {
                arrayWeaponSource.Weapons = await metadataService.GetWeaponArrayAsync(token).ConfigureAwait(false);
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

                if (context is IMetadataDictionaryIdAvatarWithPlayersSource)
                {
                    dictionaryIdAvatarSource.IdAvatarMap = AvatarIds.WithPlayers(dictionaryIdAvatarSource.IdAvatarMap);
                }
            }

            if (context is IMetadataDictionaryIdDictionaryLevelAvatarPromoteSource dictionaryIdDictionaryLevelAvatarPromoteSource)
            {
                dictionaryIdDictionaryLevelAvatarPromoteSource.IdDictionaryAvatarLevelPromoteMap = await metadataService.GetIdToAvatarPromoteGroupMapAsync(token).ConfigureAwait(false);
            }

            if (context is IMetadataDictionaryIdDictionaryLevelWeaponPromoteSource dictionaryIdDictionaryLevelWeaponPromoteSource)
            {
                dictionaryIdDictionaryLevelWeaponPromoteSource.IdDictionaryWeaponLevelPromoteMap = await metadataService.GetIdToWeaponPromoteGroupMapAsync(token).ConfigureAwait(false);
            }

            if (context is IMetadataDictionaryIdArrayTowerLevelSource dictionaryIdListTowerLevelSource)
            {
                dictionaryIdListTowerLevelSource.IdArrayTowerLevelMap = await metadataService.GetGroupIdToTowerLevelGroupMapAsync(token).ConfigureAwait(false);
            }

            if (context is IMetadataDictionaryIdDisplayItemAndMaterialSource dictionaryIdDisplayItemAndMaterialSource)
            {
                dictionaryIdDisplayItemAndMaterialSource.IdDisplayItemAndMaterialMap = await metadataService.GetIdToDisplayItemAndMaterialMapAsync(token).ConfigureAwait(false);
            }

            if (context is IMetadataDictionaryIdMaterialSource dictionaryIdMaterialSource)
            {
                dictionaryIdMaterialSource.IdMaterialMap = await metadataService.GetIdToMaterialMapAsync(token).ConfigureAwait(false);
            }

            if (context is IMetadataDictionaryIdMonsterSource dictionaryIdMonsterSource)
            {
                dictionaryIdMonsterSource.IdMonsterMap = await metadataService.GetRelationshipIdToMonsterMapAsync(token).ConfigureAwait(false);
            }

            if (context is IMetadataDictionaryIdReliquarySource dictionaryIdReliquarySource)
            {
                dictionaryIdReliquarySource.IdReliquaryMap = await metadataService.GetIdToReliquaryMapAsync(token).ConfigureAwait(false);
            }

            if (context is IMetadataDictionaryIdReliquaryMainPropertySource dictionaryIdReliquaryMainPropertySource)
            {
                dictionaryIdReliquaryMainPropertySource.IdReliquaryMainPropertyMap = await metadataService.GetIdToReliquaryMainPropertyMapAsync(token).ConfigureAwait(false);
            }

            if (context is IMetadataDictionaryIdReliquarySetSource dictionaryIdReliquarySetSource)
            {
                dictionaryIdReliquarySetSource.IdReliquarySetMap = await metadataService.GetIdToReliquarySetMapAsync(token).ConfigureAwait(false);
            }

            if (context is IMetadataDictionaryIdReliquarySubAffixSource dictionaryIdReliquarySubAffixSource)
            {
                dictionaryIdReliquarySubAffixSource.IdReliquarySubAffixMap = await metadataService.GetIdToReliquarySubAffixMapAsync(token).ConfigureAwait(false);
            }

            if (context is IMetadataDictionaryIdRoleCombatScheduleSource dictionaryIdRoleCombatScheduleSource)
            {
                dictionaryIdRoleCombatScheduleSource.IdRoleCombatScheduleMap = await metadataService.GetIdToRoleCombatScheduleMapAsync(token).ConfigureAwait(false);
            }

            if (context is IMetadataDictionaryIdTowerFloorSource dictionaryIdTowerFloorSource)
            {
                dictionaryIdTowerFloorSource.IdTowerFloorMap = await metadataService.GetIdToTowerFloorMapAsync(token).ConfigureAwait(false);
            }

            if (context is IMetadataDictionaryIdTowerScheduleSource dictionaryIdTowerScheduleSource)
            {
                dictionaryIdTowerScheduleSource.IdTowerScheduleMap = await metadataService.GetIdToTowerScheduleMapAsync(token).ConfigureAwait(false);
            }

            if (context is IMetadataDictionaryIdWeaponSource dictionaryIdWeaponSource)
            {
                dictionaryIdWeaponSource.IdWeaponMap = await metadataService.GetIdToWeaponMapAsync(token).ConfigureAwait(false);
            }

            if (context is IMetadataDictionaryLevelAvaterGrowCurveSource levelAvaterGrowCurveSource)
            {
                levelAvaterGrowCurveSource.LevelDictionaryAvatarGrowCurveMap = await metadataService.GetLevelToAvatarCurveMapAsync(token).ConfigureAwait(false);
            }

            if (context is IMetadataDictionaryLevelMonsterGrowCurveSource monsterGrowCurveSource)
            {
                monsterGrowCurveSource.LevelDictionaryMonsterGrowCurveMap = await metadataService.GetLevelToMonsterCurveMapAsync(token).ConfigureAwait(false);
            }

            if (context is IMetadataDictionaryLevelWeaponGrowCurveSource weaponGrowCurveSource)
            {
                weaponGrowCurveSource.LevelDictionaryWeaponGrowCurveMap = await metadataService.GetLevelToWeaponCurveMapAsync(token).ConfigureAwait(false);
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

    public static IEnumerable<Material> EnumerateInventoryMaterial(this IMetadataArrayMaterialSource context)
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
}