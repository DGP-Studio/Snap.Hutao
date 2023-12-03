// Copyright (c) DGP Studio. All rights reserved.
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
using static Snap.Hutao.Service.Metadata.MetadataFileNames;

namespace Snap.Hutao.Service.Metadata;

internal static class MetadataServiceDictionaryExtension
{
    public static ValueTask<Dictionary<EquipAffixId, ReliquarySet>> GetEquipAffixIdToReliquarySetMapAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        return metadataService.FromCacheAsDictionaryAsync<EquipAffixId, ReliquarySet>(FileNameReliquarySet, r => r.EquipAffixId, token);
    }

    public static ValueTask<Dictionary<ExtendedEquipAffixId, ReliquarySet>> GetExtendedEquipAffixIdToReliquarySetMapAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        return metadataService.FromCacheAsDictionaryAsync<ReliquarySet, (ExtendedEquipAffixId Id, ReliquarySet Set), ExtendedEquipAffixId, ReliquarySet>(
            FileNameReliquarySet,
            list => list.SelectMany(set => set.EquipAffixIds.Select(id => (Id: id, Set: set))),
            tuple => tuple.Id,
            tuple => tuple.Set,
            token);
    }

    public static ValueTask<Dictionary<TowerLevelGroupId, List<TowerLevel>>> GetGroupIdToTowerLevelGroupMapAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        return metadataService.FromCacheAsDictionaryAsync<TowerLevel, IGrouping<TowerLevelGroupId, TowerLevel>, TowerLevelGroupId, List<TowerLevel>>(
            FileNameTowerLevel,
            list => list.GroupBy(l => l.GroupId),
            g => g.Key,
            g => g.ToList(),
            token);
    }

    public static ValueTask<Dictionary<AchievementId, Model.Metadata.Achievement.Achievement>> GetIdToAchievementMapAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        return metadataService.FromCacheAsDictionaryAsync<AchievementId, Model.Metadata.Achievement.Achievement>(FileNameAchievement, a => a.Id, token);
    }

    public static ValueTask<Dictionary<AvatarId, Avatar>> GetIdToAvatarMapAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        return metadataService.FromCacheAsDictionaryAsync<AvatarId, Avatar>(FileNameAvatar, a => a.Id, token);
    }

    public static async ValueTask<Dictionary<MaterialId, DisplayItem>> GetIdToDisplayItemAndMaterialMapAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        string cacheKey = $"{nameof(MetadataService)}.Cache.{FileNameDisplayItem}+{FileNameMaterial}.Map.{typeof(MaterialId).Name}.{nameof(DisplayItem)}+{nameof(Material)}";

        if (metadataService.MemoryCache.TryGetValue(cacheKey, out object? value))
        {
            ArgumentNullException.ThrowIfNull(value);
            return (Dictionary<MaterialId, DisplayItem>)value;
        }

        Dictionary<MaterialId, DisplayItem> displays = await metadataService.FromCacheAsDictionaryAsync<MaterialId, DisplayItem>(FileNameDisplayItem, a => a.Id, token).ConfigureAwait(false);
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
        return metadataService.FromCacheAsDictionaryAsync<MaterialId, Material>(FileNameMaterial, a => a.Id, token);
    }

    public static ValueTask<Dictionary<AvatarId, ReliquaryAffixWeight>> GetIdToReliquaryAffixWeightMapAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        return metadataService.FromCacheAsDictionaryAsync<AvatarId, ReliquaryAffixWeight>(FileNameReliquaryAffixWeight, r => r.AvatarId, token);
    }

    public static ValueTask<Dictionary<ReliquaryMainAffixId, FightProperty>> GetIdToReliquaryMainPropertyMapAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        return metadataService.FromCacheAsDictionaryAsync<ReliquaryMainAffix, ReliquaryMainAffixId, FightProperty>(FileNameReliquaryMainAffix, r => r.Id, r => r.Type, token);
    }

    public static ValueTask<Dictionary<ReliquarySubAffixId, ReliquarySubAffix>> GetIdToReliquarySubAffixMapAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        return metadataService.FromCacheAsDictionaryAsync<ReliquarySubAffixId, ReliquarySubAffix>(FileNameReliquarySubAffix, a => a.Id, token);
    }

    public static ValueTask<Dictionary<TowerFloorId, TowerFloor>> GetIdToTowerFloorMapAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        return metadataService.FromCacheAsDictionaryAsync<TowerFloorId, TowerFloor>(FileNameTowerFloor, t => t.Id, token);
    }

    public static ValueTask<Dictionary<TowerScheduleId, TowerSchedule>> GetIdToTowerScheduleMapAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        return metadataService.FromCacheAsDictionaryAsync<TowerScheduleId, TowerSchedule>(FileNameTowerSchedule, t => t.Id, token);
    }

    public static ValueTask<Dictionary<WeaponId, Weapon>> GetIdToWeaponMapAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        return metadataService.FromCacheAsDictionaryAsync<WeaponId, Weapon>(FileNameWeapon, w => w.Id, token);
    }

    public static ValueTask<Dictionary<Level, Dictionary<GrowCurveType, float>>> GetLevelToAvatarCurveMapAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        return metadataService.FromCacheAsDictionaryAsync<GrowCurve, Level, Dictionary<GrowCurveType, float>>(FileNameAvatarCurve, a => a.Level, a => a.Map, token);
    }

    public static ValueTask<Dictionary<Level, Dictionary<GrowCurveType, float>>> GetLevelToMonsterCurveMapAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        return metadataService.FromCacheAsDictionaryAsync<GrowCurve, Level, Dictionary<GrowCurveType, float>>(FileNameMonsterCurve, m => m.Level, m => m.Map, token);
    }

    public static ValueTask<Dictionary<Level, Dictionary<GrowCurveType, float>>> GetLevelToWeaponCurveMapAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        return metadataService.FromCacheAsDictionaryAsync<GrowCurve, Level, Dictionary<GrowCurveType, float>>(FileNameWeaponCurve, w => w.Level, w => w.Map, token);
    }

    public static ValueTask<Dictionary<string, Avatar>> GetNameToAvatarMapAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        return metadataService.FromCacheAsDictionaryAsync<string, Avatar>(FileNameAvatar, a => a.Name, token);
    }

    public static ValueTask<Dictionary<string, Weapon>> GetNameToWeaponMapAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        return metadataService.FromCacheAsDictionaryAsync<string, Weapon>(FileNameWeapon, w => w.Name, token);
    }

    public static ValueTask<Dictionary<MonsterRelationshipId, Monster>> GetRelationshipIdToMonsterMapAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        return metadataService.FromCacheAsDictionaryAsync<MonsterRelationshipId, Monster>(FileNameMonster, m => m.RelationshipId, token);
    }

    private static async ValueTask<Dictionary<TKey, TValue>> FromCacheAsDictionaryAsync<TKey, TValue>(this IMetadataService metadataService, string fileName, Func<TValue, TKey> keySelector, CancellationToken token)
        where TKey : notnull
    {
        string keyName = TypeNameHelper.GetTypeDisplayName(typeof(TKey));
        string valueName = TypeNameHelper.GetTypeDisplayName(typeof(TValue));
        string cacheKey = $"{nameof(MetadataService)}.Cache.{fileName}.Map.{keyName}.{valueName}";

        if (metadataService.MemoryCache.TryGetValue(cacheKey, out object? value))
        {
            ArgumentNullException.ThrowIfNull(value);
            return (Dictionary<TKey, TValue>)value;
        }

        List<TValue> list = await metadataService.FromCacheOrFileAsync<List<TValue>>(fileName, token).ConfigureAwait(false);
        Dictionary<TKey, TValue> dict = list.ToDictionaryIgnoringDuplicateKeys(keySelector); // There are duplicate name items
        return metadataService.MemoryCache.Set(cacheKey, dict);
    }

    private static async ValueTask<Dictionary<TKey, TValue>> FromCacheAsDictionaryAsync<TData, TKey, TValue>(this IMetadataService metadataService, string fileName, Func<TData, TKey> keySelector, Func<TData, TValue> valueSelector, CancellationToken token)
        where TKey : notnull
    {
        string keyName = TypeNameHelper.GetTypeDisplayName(typeof(TKey));
        string valueName = TypeNameHelper.GetTypeDisplayName(typeof(TValue));
        string cacheKey = $"{nameof(MetadataService)}.Cache.{fileName}.Map.{keyName}.{valueName}";

        if (metadataService.MemoryCache.TryGetValue(cacheKey, out object? value))
        {
            ArgumentNullException.ThrowIfNull(value);
            return (Dictionary<TKey, TValue>)value;
        }

        List<TData> list = await metadataService.FromCacheOrFileAsync<List<TData>>(fileName, token).ConfigureAwait(false);
        Dictionary<TKey, TValue> dict = list.ToDictionaryIgnoringDuplicateKeys(keySelector, valueSelector); // There are duplicate name items
        return metadataService.MemoryCache.Set(cacheKey, dict);
    }

    private static async ValueTask<Dictionary<TKey, TValue>> FromCacheAsDictionaryAsync<TData, TMiddle, TKey, TValue>(this IMetadataService metadataService, string fileName, Func<List<TData>, IEnumerable<TMiddle>> listSelector, Func<TMiddle, TKey> keySelector, Func<TMiddle, TValue> valueSelector, CancellationToken token)
        where TKey : notnull
    {
        string keyName = TypeNameHelper.GetTypeDisplayName(typeof(TKey));
        string middleName = TypeNameHelper.GetTypeDisplayName(typeof(TMiddle));
        string valueName = TypeNameHelper.GetTypeDisplayName(typeof(TValue));
        string cacheKey = $"{nameof(MetadataService)}.Cache.{fileName}.Map.{keyName}.{middleName}.{valueName}";

        if (metadataService.MemoryCache.TryGetValue(cacheKey, out object? value))
        {
            ArgumentNullException.ThrowIfNull(value);
            return (Dictionary<TKey, TValue>)value;
        }

        List<TData> list = await metadataService.FromCacheOrFileAsync<List<TData>>(fileName, token).ConfigureAwait(false);
        Dictionary<TKey, TValue> dict = listSelector(list).ToDictionaryIgnoringDuplicateKeys(keySelector, valueSelector); // There are duplicate name items
        return metadataService.MemoryCache.Set(cacheKey, dict);
    }
}