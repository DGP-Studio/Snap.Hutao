﻿// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Extensions.Caching.Memory;
using Snap.Hutao.Core;
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Metadata;
using Snap.Hutao.Model.Metadata.Avatar;
using Snap.Hutao.Model.Metadata.Item;
using Snap.Hutao.Model.Metadata.Monster;
using Snap.Hutao.Model.Metadata.Reliquary;
using Snap.Hutao.Model.Metadata.Tower;
using Snap.Hutao.Model.Metadata.Weapon;
using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Service.Metadata;

internal static class MetadataServiceDictionaryExtension
{
    public static ValueTask<Dictionary<EquipAffixId, ReliquarySet>> GetEquipAffixIdToReliquarySetMapAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        return metadataService.FromCacheAsDictionaryAsync<EquipAffixId, ReliquarySet>(MetadataFileStrategies.ReliquarySet, r => r.EquipAffixId, token);
    }

    public static ValueTask<Dictionary<ExtendedEquipAffixId, ReliquarySet>> GetExtendedEquipAffixIdToReliquarySetMapAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        return metadataService.FromCacheAsDictionaryAsync(MetadataFileStrategies.ReliquarySet, (List<ReliquarySet> list) => list.SelectMany(set => set.EquipAffixIds, (set, id) => (Id: id, Set: set)), token);
    }

    public static ValueTask<Dictionary<TowerLevelGroupId, List<TowerLevel>>> GetGroupIdToTowerLevelGroupMapAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        return metadataService.FromCacheAsDictionaryAsync(MetadataFileStrategies.TowerLevel, (List<TowerLevel> list) => list.GroupBy(l => l.GroupId), g => g.Key, g => g.ToList(), token);
    }

    public static ValueTask<Dictionary<AchievementId, Model.Metadata.Achievement.Achievement>> GetIdToAchievementMapAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        return metadataService.FromCacheAsDictionaryAsync<AchievementId, Model.Metadata.Achievement.Achievement>(MetadataFileStrategies.Achievement, a => a.Id, token);
    }

    public static ValueTask<Dictionary<AvatarId, Avatar>> GetIdToAvatarMapAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        return metadataService.FromCacheAsDictionaryAsync<AvatarId, Avatar>(MetadataFileStrategies.Avatar, a => a.Id, token);
    }

    public static ValueTask<Dictionary<PromoteId, Dictionary<PromoteLevel, Promote>>> GetIdToAvatarPromoteGroupMapAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        return metadataService.FromCacheAsDictionaryAsync(MetadataFileStrategies.AvatarPromote, (List<Promote> list) => list.GroupBy(p => p.Id), g => g.Key, g => g.ToDictionary(p => p.Level), token);
    }

    public static async ValueTask<Dictionary<MaterialId, DisplayItem>> GetIdToDisplayItemAndMaterialMapAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        string cacheKey = $"{nameof(MetadataService)}.Cache.{nameof(MetadataFileStrategies.DisplayItem)}+{nameof(MetadataFileStrategies.Material)}.Map.{typeof(MaterialId).Name}.{nameof(DisplayItem)}+{nameof(Material)}";

        if (metadataService.MemoryCache.TryGetValue(cacheKey, out object? value))
        {
            ArgumentNullException.ThrowIfNull(value);
            return (Dictionary<MaterialId, DisplayItem>)value;
        }

        Dictionary<MaterialId, DisplayItem> displays = await metadataService.FromCacheAsDictionaryAsync<MaterialId, DisplayItem>(MetadataFileStrategies.DisplayItem, a => a.Id, token).ConfigureAwait(false);
        Dictionary<MaterialId, Material> materials = await metadataService.GetIdToMaterialMapAsync(token).ConfigureAwait(false);

        Dictionary<MaterialId, DisplayItem> results = new(displays);

        foreach ((MaterialId id, DisplayItem material) in materials)
        {
            results[id] = material;
        }

        return metadataService.MemoryCache.Set(cacheKey, results);
    }

    public static ValueTask<Dictionary<MaterialId, Material>> GetIdToMaterialMapAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        return metadataService.FromCacheAsDictionaryAsync<MaterialId, Material>(MetadataFileStrategies.Material, a => a.Id, token);
    }

    public static ValueTask<Dictionary<ReliquaryId, Reliquary>> GetIdToReliquaryMapAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        return metadataService.FromCacheAsDictionaryAsync(MetadataFileStrategies.Reliquary, (List<Reliquary> list) => list.SelectMany(r => r.Ids, (r, i) => (Index: i, Reliquary: r)), token);
    }

    public static ValueTask<Dictionary<AvatarId, ReliquaryAffixWeight>> GetIdToReliquaryAffixWeightMapAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        return metadataService.FromCacheAsDictionaryAsync<AvatarId, ReliquaryAffixWeight>(MetadataFileStrategies.ReliquaryAffixWeight, r => r.AvatarId, token);
    }

    public static ValueTask<Dictionary<ReliquaryMainAffixId, FightProperty>> GetIdToReliquaryMainPropertyMapAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        return metadataService.FromCacheAsDictionaryAsync<ReliquaryMainAffix, ReliquaryMainAffixId, FightProperty>(MetadataFileStrategies.ReliquaryMainAffix, r => r.Id, r => r.Type, token);
    }

    public static ValueTask<Dictionary<ReliquarySetId, ReliquarySet>> GetIdToReliquarySetMapAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        return metadataService.FromCacheAsDictionaryAsync<ReliquarySetId, ReliquarySet>(MetadataFileStrategies.ReliquarySet, r => r.SetId, token);
    }

    public static ValueTask<Dictionary<ReliquarySubAffixId, ReliquarySubAffix>> GetIdToReliquarySubAffixMapAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        return metadataService.FromCacheAsDictionaryAsync<ReliquarySubAffixId, ReliquarySubAffix>(MetadataFileStrategies.ReliquarySubAffix, a => a.Id, token);
    }

    public static ValueTask<Dictionary<TowerFloorId, TowerFloor>> GetIdToTowerFloorMapAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        return metadataService.FromCacheAsDictionaryAsync<TowerFloorId, TowerFloor>(MetadataFileStrategies.TowerFloor, t => t.Id, token);
    }

    public static ValueTask<Dictionary<TowerScheduleId, TowerSchedule>> GetIdToTowerScheduleMapAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        return metadataService.FromCacheAsDictionaryAsync<TowerScheduleId, TowerSchedule>(MetadataFileStrategies.TowerSchedule, t => t.Id, token);
    }

    public static ValueTask<Dictionary<WeaponId, Weapon>> GetIdToWeaponMapAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        return metadataService.FromCacheAsDictionaryAsync<WeaponId, Weapon>(MetadataFileStrategies.Weapon, w => w.Id, token);
    }

    public static ValueTask<Dictionary<PromoteId, Dictionary<PromoteLevel, Promote>>> GetIdToWeaponPromoteGroupMapAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        return metadataService.FromCacheAsDictionaryAsync(MetadataFileStrategies.WeaponPromote, (List<Promote> list) => list.GroupBy(p => p.Id), g => g.Key, g => g.ToDictionary(p => p.Level), token);
    }

    public static ValueTask<Dictionary<Level, Dictionary<GrowCurveType, float>>> GetLevelToAvatarCurveMapAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        return metadataService.FromCacheAsDictionaryAsync<GrowCurve, Level, Dictionary<GrowCurveType, float>>(MetadataFileStrategies.AvatarCurve, a => a.Level, a => a.Map, token);
    }

    public static ValueTask<Dictionary<Level, Dictionary<GrowCurveType, float>>> GetLevelToMonsterCurveMapAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        return metadataService.FromCacheAsDictionaryAsync<GrowCurve, Level, Dictionary<GrowCurveType, float>>(MetadataFileStrategies.MonsterCurve, m => m.Level, m => m.Map, token);
    }

    public static ValueTask<Dictionary<Level, Dictionary<GrowCurveType, float>>> GetLevelToWeaponCurveMapAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        return metadataService.FromCacheAsDictionaryAsync<GrowCurve, Level, Dictionary<GrowCurveType, float>>(MetadataFileStrategies.WeaponCurve, w => w.Level, w => w.Map, token);
    }

    public static ValueTask<Dictionary<string, Avatar>> GetNameToAvatarMapAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        return metadataService.FromCacheAsDictionaryAsync<string, Avatar>(MetadataFileStrategies.Avatar, a => a.Name, token);
    }

    public static ValueTask<Dictionary<string, Weapon>> GetNameToWeaponMapAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        return metadataService.FromCacheAsDictionaryAsync<string, Weapon>(MetadataFileStrategies.Weapon, w => w.Name, token);
    }

    public static ValueTask<Dictionary<MonsterRelationshipId, Monster>> GetRelationshipIdToMonsterMapAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        return metadataService.FromCacheAsDictionaryAsync<MonsterRelationshipId, Monster>(MetadataFileStrategies.Monster, m => m.RelationshipId, token);
    }

    private static async ValueTask<Dictionary<TKey, TValue>> FromCacheAsDictionaryAsync<TKey, TValue>(this IMetadataService metadataService, MetadataFileStrategy strategy, Func<TValue, TKey> keySelector, CancellationToken token)
        where TKey : notnull
        where TValue : class
    {
        string keyName = TypeNameHelper.GetTypeDisplayName(typeof(TKey));
        string valueName = TypeNameHelper.GetTypeDisplayName(typeof(TValue));
        string cacheKey = $"{nameof(MetadataService)}.Cache.{strategy.Name}.Map.{keyName}.{valueName}";

        if (metadataService.MemoryCache.TryGetValue(cacheKey, out object? value))
        {
            ArgumentNullException.ThrowIfNull(value);
            return (Dictionary<TKey, TValue>)value;
        }

        List<TValue> list = await metadataService.FromCacheOrFileAsync<TValue>(strategy, token).ConfigureAwait(false);
        Dictionary<TKey, TValue> dict = list.ToDictionaryIgnoringDuplicateKeys(keySelector); // There are duplicate name items
        return metadataService.MemoryCache.Set(cacheKey, dict);
    }

    private static async ValueTask<Dictionary<TKey, TValue>> FromCacheAsDictionaryAsync<TData, TKey, TValue>(this IMetadataService metadataService, MetadataFileStrategy strategy, Func<TData, TKey> keySelector, Func<TData, TValue> valueSelector, CancellationToken token)
        where TKey : notnull
        where TData : class
    {
        string keyName = TypeNameHelper.GetTypeDisplayName(typeof(TKey));
        string valueName = TypeNameHelper.GetTypeDisplayName(typeof(TValue));
        string cacheKey = $"{nameof(MetadataService)}.Cache.{strategy.Name}.Map.{keyName}.{valueName}";
        if (metadataService.MemoryCache.TryGetValue(cacheKey, out object? value))
        {
            ArgumentNullException.ThrowIfNull(value);
            return (Dictionary<TKey, TValue>)value;
        }

        List<TData> list = await metadataService.FromCacheOrFileAsync<TData>(strategy, token).ConfigureAwait(false);
        Dictionary<TKey, TValue> dict = list.ToDictionaryIgnoringDuplicateKeys(keySelector, valueSelector); // There are duplicate name items
        return metadataService.MemoryCache.Set(cacheKey, dict);
    }

    private static ValueTask<Dictionary<TKey, TValue>> FromCacheAsDictionaryAsync<TData, TKey, TValue>(this IMetadataService metadataService, MetadataFileStrategy strategy, Func<List<TData>, IEnumerable<(TKey Key, TValue Value)>> listSelector, CancellationToken token)
        where TKey : notnull
        where TData : class
    {
        return FromCacheAsDictionaryAsync(metadataService, strategy, listSelector, kvp => kvp.Key, kvp => kvp.Value, token);
    }

    private static async ValueTask<Dictionary<TKey, TValue>> FromCacheAsDictionaryAsync<TData, TMiddle, TKey, TValue>(this IMetadataService metadataService, MetadataFileStrategy strategy, Func<List<TData>, IEnumerable<TMiddle>> listSelector, Func<TMiddle, TKey> keySelector, Func<TMiddle, TValue> valueSelector, CancellationToken token)
        where TKey : notnull
        where TData : class
    {
        string keyName = TypeNameHelper.GetTypeDisplayName(typeof(TKey));
        string middleName = TypeNameHelper.GetTypeDisplayName(typeof(TMiddle));
        string valueName = TypeNameHelper.GetTypeDisplayName(typeof(TValue));
        string cacheKey = $"{nameof(MetadataService)}.Cache.{strategy.Name}.Map.{keyName}.{middleName}.{valueName}";

        if (metadataService.MemoryCache.TryGetValue(cacheKey, out object? value))
        {
            ArgumentNullException.ThrowIfNull(value);
            return (Dictionary<TKey, TValue>)value;
        }

        List<TData> list = await metadataService.FromCacheOrFileAsync<TData>(strategy, token).ConfigureAwait(false);
        Dictionary<TKey, TValue> dict = listSelector(list).ToDictionaryIgnoringDuplicateKeys(keySelector, valueSelector); // There are duplicate name items
        return metadataService.MemoryCache.Set(cacheKey, dict);
    }
}