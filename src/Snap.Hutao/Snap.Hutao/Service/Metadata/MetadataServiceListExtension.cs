// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Metadata;
using Snap.Hutao.Model.Metadata.Achievement;
using Snap.Hutao.Model.Metadata.Avatar;
using Snap.Hutao.Model.Metadata.Item;
using Snap.Hutao.Model.Metadata.Monster;
using Snap.Hutao.Model.Metadata.Reliquary;
using Snap.Hutao.Model.Metadata.Tower;
using Snap.Hutao.Model.Metadata.Weapon;
using static Snap.Hutao.Service.Metadata.MetadataFileNames;

namespace Snap.Hutao.Service.Metadata;

internal static class MetadataServiceListExtension
{
    public static ValueTask<List<Model.Metadata.Achievement.Achievement>> GetAchievementListAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        return metadataService.FromCacheOrFileAsync<List<Model.Metadata.Achievement.Achievement>>(FileNameAchievement, token);
    }

    public static ValueTask<List<Chapter>> GetChapterListAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        return metadataService.FromCacheOrFileAsync<List<Chapter>>(FileNameChapter, token);
    }

    public static ValueTask<List<AchievementGoal>> GetAchievementGoalListAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        return metadataService.FromCacheOrFileAsync<List<AchievementGoal>>(FileNameAchievementGoal, token);
    }

    public static ValueTask<List<Avatar>> GetAvatarListAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        return metadataService.FromCacheOrFileAsync<List<Avatar>>(FileNameAvatar, token);
    }

    public static ValueTask<List<GrowCurve>> GetAvatarCurveListAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        return metadataService.FromCacheOrFileAsync<List<GrowCurve>>(FileNameAvatarCurve, token);
    }

    public static ValueTask<List<Promote>> GetAvatarPromoteListAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        return metadataService.FromCacheOrFileAsync<List<Promote>>(FileNameAvatarPromote, token);
    }

    public static ValueTask<List<DisplayItem>> GetDisplayItemListAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        return metadataService.FromCacheOrFileAsync<List<DisplayItem>>(FileNameDisplayItem, token);
    }

    public static ValueTask<List<GachaEvent>> GetGachaEventListAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        return metadataService.FromCacheOrFileAsync<List<GachaEvent>>(FileNameGachaEvent, token);
    }

    public static ValueTask<List<Material>> GetMaterialListAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        return metadataService.FromCacheOrFileAsync<List<Material>>(FileNameMaterial, token);
    }

    public static ValueTask<List<Monster>> GetMonsterListAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        return metadataService.FromCacheOrFileAsync<List<Monster>>(FileNameMonster, token);
    }

    public static ValueTask<List<GrowCurve>> GetMonsterCurveListAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        return metadataService.FromCacheOrFileAsync<List<GrowCurve>>(FileNameMonsterCurve, token);
    }

    public static ValueTask<List<ProfilePicture>> GetProfilePictureListAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        return metadataService.FromCacheOrFileAsync<List<ProfilePicture>>(FileNameProfilePicture, token);
    }

    public static ValueTask<List<Reliquary>> GetReliquaryListAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        return metadataService.FromCacheOrFileAsync<List<Reliquary>>(FileNameReliquary, token);
    }

    public static ValueTask<List<ReliquaryAffixWeight>> GetReliquaryAffixWeightListAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        return metadataService.FromCacheOrFileAsync<List<ReliquaryAffixWeight>>(FileNameReliquaryAffixWeight, token);
    }

    public static ValueTask<List<ReliquaryMainAffix>> GetReliquaryMainAffixListAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        return metadataService.FromCacheOrFileAsync<List<ReliquaryMainAffix>>(FileNameReliquaryMainAffix, token);
    }

    public static ValueTask<List<ReliquaryMainAffixLevel>> GetReliquaryMainAffixLevelListAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        return metadataService.FromCacheOrFileAsync<List<ReliquaryMainAffixLevel>>(FileNameReliquaryMainAffixLevel, token);
    }

    public static ValueTask<List<ReliquarySet>> GetReliquarySetListAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        return metadataService.FromCacheOrFileAsync<List<ReliquarySet>>(FileNameReliquarySet, token);
    }

    public static ValueTask<List<ReliquarySubAffix>> GetReliquarySubAffixListAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        return metadataService.FromCacheOrFileAsync<List<ReliquarySubAffix>>(FileNameReliquarySubAffix, token);
    }

    public static ValueTask<List<TowerFloor>> GetTowerFloorListAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        return metadataService.FromCacheOrFileAsync<List<TowerFloor>>(FileNameTowerFloor, token);
    }

    public static ValueTask<List<TowerLevel>> GetTowerLevelListAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        return metadataService.FromCacheOrFileAsync<List<TowerLevel>>(FileNameTowerLevel, token);
    }

    public static ValueTask<List<TowerSchedule>> GetTowerScheduleListAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        return metadataService.FromCacheOrFileAsync<List<TowerSchedule>>(FileNameTowerSchedule, token);
    }

    public static ValueTask<List<Weapon>> GetWeaponListAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        return metadataService.FromCacheOrFileAsync<List<Weapon>>(FileNameWeapon, token);
    }

    public static ValueTask<List<GrowCurve>> GetWeaponCurveListAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        return metadataService.FromCacheOrFileAsync<List<GrowCurve>>(FileNameWeaponCurve, token);
    }

    public static ValueTask<List<Promote>> GetWeaponPromoteListAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        return metadataService.FromCacheOrFileAsync<List<Promote>>(FileNameWeaponPromote, token);
    }
}