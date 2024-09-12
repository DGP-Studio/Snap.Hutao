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
using System.Collections.Immutable;

namespace Snap.Hutao.Service.Metadata;

internal static class MetadataServiceImmutableArrayExtension
{
    public static ValueTask<ImmutableArray<Model.Metadata.Achievement.Achievement>> GetAchievementListAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        return metadataService.FromCacheOrFileAsync<Model.Metadata.Achievement.Achievement>(MetadataFileStrategies.Achievement, token);
    }

    public static ValueTask<ImmutableArray<Chapter>> GetChapterListAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        return metadataService.FromCacheOrFileAsync<Chapter>(MetadataFileStrategies.Chapter, token);
    }

    public static ValueTask<ImmutableArray<AchievementGoal>> GetAchievementGoalListAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        return metadataService.FromCacheOrFileAsync<AchievementGoal>(MetadataFileStrategies.AchievementGoal, token);
    }

    public static ValueTask<ImmutableArray<Avatar>> GetAvatarListAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        return metadataService.FromCacheOrFileAsync<Avatar>(MetadataFileStrategies.Avatar, token);
    }

    public static ValueTask<ImmutableArray<GrowCurve>> GetAvatarCurveListAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        return metadataService.FromCacheOrFileAsync<GrowCurve>(MetadataFileStrategies.AvatarCurve, token);
    }

    public static ValueTask<ImmutableArray<Promote>> GetAvatarPromoteListAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        return metadataService.FromCacheOrFileAsync<Promote>(MetadataFileStrategies.AvatarPromote, token);
    }

    public static ValueTask<ImmutableArray<DisplayItem>> GetDisplayItemListAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        return metadataService.FromCacheOrFileAsync<DisplayItem>(MetadataFileStrategies.DisplayItem, token);
    }

    public static ValueTask<ImmutableArray<GachaEvent>> GetGachaEventListAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        return metadataService.FromCacheOrFileAsync<GachaEvent>(MetadataFileStrategies.GachaEvent, token);
    }

    public static ValueTask<ImmutableArray<Material>> GetMaterialListAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        return metadataService.FromCacheOrFileAsync<Material>(MetadataFileStrategies.Material, token);
    }

    public static ValueTask<ImmutableArray<Monster>> GetMonsterListAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        return metadataService.FromCacheOrFileAsync<Monster>(MetadataFileStrategies.Monster, token);
    }

    public static ValueTask<ImmutableArray<GrowCurve>> GetMonsterCurveListAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        return metadataService.FromCacheOrFileAsync<GrowCurve>(MetadataFileStrategies.MonsterCurve, token);
    }

    public static ValueTask<ImmutableArray<ProfilePicture>> GetProfilePictureListAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        return metadataService.FromCacheOrFileAsync<ProfilePicture>(MetadataFileStrategies.ProfilePicture, token);
    }

    public static ValueTask<ImmutableArray<Reliquary>> GetReliquaryListAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        return metadataService.FromCacheOrFileAsync<Reliquary>(MetadataFileStrategies.Reliquary, token);
    }

    public static ValueTask<ImmutableArray<ReliquaryMainAffix>> GetReliquaryMainAffixListAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        return metadataService.FromCacheOrFileAsync<ReliquaryMainAffix>(MetadataFileStrategies.ReliquaryMainAffix, token);
    }

    public static ValueTask<ImmutableArray<ReliquaryMainAffixLevel>> GetReliquaryMainAffixLevelListAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        return metadataService.FromCacheOrFileAsync<ReliquaryMainAffixLevel>(MetadataFileStrategies.ReliquaryMainAffixLevel, token);
    }

    public static ValueTask<ImmutableArray<ReliquarySet>> GetReliquarySetListAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        return metadataService.FromCacheOrFileAsync<ReliquarySet>(MetadataFileStrategies.ReliquarySet, token);
    }

    public static ValueTask<ImmutableArray<ReliquarySubAffix>> GetReliquarySubAffixListAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        return metadataService.FromCacheOrFileAsync<ReliquarySubAffix>(MetadataFileStrategies.ReliquarySubAffix, token);
    }

    public static ValueTask<ImmutableArray<TowerFloor>> GetTowerFloorListAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        return metadataService.FromCacheOrFileAsync<TowerFloor>(MetadataFileStrategies.TowerFloor, token);
    }

    public static ValueTask<ImmutableArray<TowerLevel>> GetTowerLevelListAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        return metadataService.FromCacheOrFileAsync<TowerLevel>(MetadataFileStrategies.TowerLevel, token);
    }

    public static ValueTask<ImmutableArray<TowerSchedule>> GetTowerScheduleListAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        return metadataService.FromCacheOrFileAsync<TowerSchedule>(MetadataFileStrategies.TowerSchedule, token);
    }

    public static ValueTask<ImmutableArray<Weapon>> GetWeaponListAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        return metadataService.FromCacheOrFileAsync<Weapon>(MetadataFileStrategies.Weapon, token);
    }

    public static ValueTask<ImmutableArray<GrowCurve>> GetWeaponCurveListAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        return metadataService.FromCacheOrFileAsync<GrowCurve>(MetadataFileStrategies.WeaponCurve, token);
    }

    public static ValueTask<ImmutableArray<Promote>> GetWeaponPromoteListAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        return metadataService.FromCacheOrFileAsync<Promote>(MetadataFileStrategies.WeaponPromote, token);
    }
}