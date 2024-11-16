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
using System.Collections.Immutable;

namespace Snap.Hutao.Service.Metadata;

internal static class MetadataServiceImmutableDictionaryExtension
{
    public static ValueTask<ImmutableDictionary<EquipAffixId, ReliquarySet>> GetEquipAffixIdToReliquarySetMapAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        return metadataService.FromCacheAsDictionaryAsync<EquipAffixId, ReliquarySet>(
            MetadataFileStrategies.ReliquarySet,
            r => r.EquipAffixId,
            token);
    }

    public static ValueTask<ImmutableDictionary<ExtendedEquipAffixId, ReliquarySet>> GetExtendedEquipAffixIdToReliquarySetMapAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        return metadataService.FromCacheAsDictionaryAsync<ReliquarySet, ExtendedEquipAffixId, ReliquarySet>(
            MetadataFileStrategies.ReliquarySet,
            array => array.SelectMany(set => set.EquipAffixIds, (set, id) => (Id: id, Set: set)),
            token);
    }

    public static ValueTask<ImmutableDictionary<TowerLevelGroupId, ImmutableArray<TowerLevel>>> GetGroupIdToTowerLevelGroupMapAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        return metadataService.FromCacheAsDictionaryAsync<TowerLevel, IGrouping<TowerLevelGroupId, TowerLevel>, TowerLevelGroupId, ImmutableArray<TowerLevel>>(
            MetadataFileStrategies.TowerLevel,
            array => array.GroupBy(l => l.GroupId),
            g => g.Key,
            ImmutableArray.ToImmutableArray,
            token);
    }

    public static ValueTask<ImmutableDictionary<AchievementId, Model.Metadata.Achievement.Achievement>> GetIdToAchievementMapAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        return metadataService.FromCacheAsDictionaryAsync<AchievementId, Model.Metadata.Achievement.Achievement>(
            MetadataFileStrategies.Achievement,
            a => a.Id,
            token);
    }

    public static ValueTask<ImmutableDictionary<AvatarId, Avatar>> GetIdToAvatarMapAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        return metadataService.FromCacheAsDictionaryAsync<AvatarId, Avatar>(
            MetadataFileStrategies.Avatar,
            a => a.Id,
            token);
    }

    public static ValueTask<ImmutableDictionary<PromoteId, ImmutableDictionary<PromoteLevel, Promote>>> GetIdToAvatarPromoteGroupMapAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        return metadataService.FromCacheAsDictionaryAsync<Promote, IGrouping<PromoteId, Promote>, PromoteId, ImmutableDictionary<PromoteLevel, Promote>>(
            MetadataFileStrategies.AvatarPromote,
            array => array.GroupBy(p => p.Id),
            g => g.Key,
            g => g.ToImmutableDictionary(p => p.Level),
            token);
    }

    public static async ValueTask<ImmutableDictionary<MaterialId, DisplayItem>> GetIdToDisplayItemAndMaterialMapAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        string cacheKey = $"{nameof(MetadataService)}.Cache.{nameof(MetadataFileStrategies.DisplayItem)}+{nameof(MetadataFileStrategies.Material)}.Map.{nameof(MaterialId)}.{nameof(DisplayItem)}+{nameof(Material)}";
        ImmutableDictionary<MaterialId, DisplayItem>? result = await metadataService.MemoryCache.GetOrCreateAsync(cacheKey, async entry =>
        {
            ImmutableDictionary<MaterialId, DisplayItem> displays = await metadataService.FromCacheAsDictionaryAsync<MaterialId, DisplayItem>(MetadataFileStrategies.DisplayItem, a => a.Id, token).ConfigureAwait(false);
            ImmutableDictionary<MaterialId, Material> materials = await metadataService.GetIdToMaterialMapAsync(token).ConfigureAwait(false);

            ImmutableDictionary<MaterialId, DisplayItem>.Builder results = ImmutableDictionary.CreateBuilder<MaterialId, DisplayItem>();
            results.AddRange(displays);

            foreach ((MaterialId id, DisplayItem material) in materials)
            {
                results[id] = material;
            }

            return results.ToImmutable();
        }).ConfigureAwait(false);

        ArgumentNullException.ThrowIfNull(result);
        return result;
    }

    public static ValueTask<ImmutableDictionary<MaterialId, Material>> GetIdToMaterialMapAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        return metadataService.FromCacheAsDictionaryAsync<MaterialId, Material>(
            MetadataFileStrategies.Material,
            a => a.Id,
            token);
    }

    public static ValueTask<ImmutableDictionary<ReliquaryId, Reliquary>> GetIdToReliquaryMapAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        return metadataService.FromCacheAsDictionaryAsync<Reliquary, ReliquaryId, Reliquary>(
            MetadataFileStrategies.Reliquary,
            array => array.SelectMany(r => r.Ids, (r, i) => (Index: i, Reliquary: r)),
            token);
    }

    public static ValueTask<ImmutableDictionary<ReliquaryMainAffixId, FightProperty>> GetIdToReliquaryMainPropertyMapAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        return metadataService.FromCacheAsDictionaryAsync<ReliquaryMainAffix, ReliquaryMainAffixId, FightProperty>(
            MetadataFileStrategies.ReliquaryMainAffix,
            r => r.Id,
            r => r.Type,
            token);
    }

    public static ValueTask<ImmutableDictionary<ReliquarySetId, ReliquarySet>> GetIdToReliquarySetMapAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        return metadataService.FromCacheAsDictionaryAsync<ReliquarySetId, ReliquarySet>(
            MetadataFileStrategies.ReliquarySet,
            r => r.SetId,
            token);
    }

    public static ValueTask<ImmutableDictionary<ReliquarySubAffixId, ReliquarySubAffix>> GetIdToReliquarySubAffixMapAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        return metadataService.FromCacheAsDictionaryAsync<ReliquarySubAffixId, ReliquarySubAffix>(
            MetadataFileStrategies.ReliquarySubAffix,
            a => a.Id,
            token);
    }

    public static ValueTask<ImmutableDictionary<RoleCombatScheduleId, RoleCombatSchedule>> GetIdToRoleCombatScheduleMapAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        return metadataService.FromCacheAsDictionaryAsync<RoleCombatScheduleId, RoleCombatSchedule>(
            MetadataFileStrategies.RoleCombatSchedule,
            r => r.Id,
            token);
    }

    public static ValueTask<ImmutableDictionary<TowerFloorId, TowerFloor>> GetIdToTowerFloorMapAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        return metadataService.FromCacheAsDictionaryAsync<TowerFloorId, TowerFloor>(
            MetadataFileStrategies.TowerFloor,
            t => t.Id,
            token);
    }

    public static ValueTask<ImmutableDictionary<TowerScheduleId, TowerSchedule>> GetIdToTowerScheduleMapAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        return metadataService.FromCacheAsDictionaryAsync<TowerScheduleId, TowerSchedule>(
            MetadataFileStrategies.TowerSchedule,
            t => t.Id,
            token);
    }

    public static ValueTask<ImmutableDictionary<WeaponId, Weapon>> GetIdToWeaponMapAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        return metadataService.FromCacheAsDictionaryAsync<WeaponId, Weapon>(
            MetadataFileStrategies.Weapon,
            w => w.Id,
            token);
    }

    public static ValueTask<ImmutableDictionary<PromoteId, ImmutableDictionary<PromoteLevel, Promote>>> GetIdToWeaponPromoteGroupMapAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        return metadataService.FromCacheAsDictionaryAsync<Promote, IGrouping<PromoteId, Promote>, PromoteId, ImmutableDictionary<PromoteLevel, Promote>>(
            MetadataFileStrategies.WeaponPromote,
            array => array.GroupBy(p => p.Id),
            g => g.Key,
            g => g.ToImmutableDictionary(p => p.Level),
            token);
    }

    public static ValueTask<ImmutableDictionary<Level, TypeValueCollection<GrowCurveType, float>>> GetLevelToAvatarCurveMapAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        return metadataService.FromCacheAsDictionaryAsync<GrowCurve, Level, TypeValueCollection<GrowCurveType, float>>(
            MetadataFileStrategies.AvatarCurve,
            a => a.Level,
            a => a.Curves,
            token);
    }

    public static ValueTask<ImmutableDictionary<Level, TypeValueCollection<GrowCurveType, float>>> GetLevelToMonsterCurveMapAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        return metadataService.FromCacheAsDictionaryAsync<GrowCurve, Level, TypeValueCollection<GrowCurveType, float>>(
            MetadataFileStrategies.MonsterCurve,
            m => m.Level,
            m => m.Curves,
            token);
    }

    public static ValueTask<ImmutableDictionary<Level, TypeValueCollection<GrowCurveType, float>>> GetLevelToWeaponCurveMapAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        return metadataService.FromCacheAsDictionaryAsync<GrowCurve, Level, TypeValueCollection<GrowCurveType, float>>(
            MetadataFileStrategies.WeaponCurve,
            w => w.Level,
            w => w.Curves,
            token);
    }

    public static ValueTask<ImmutableDictionary<string, Avatar>> GetNameToAvatarMapAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        return metadataService.FromCacheAsDictionaryAsync<string, Avatar>(
            MetadataFileStrategies.Avatar,
            a => a.Name,
            token);
    }

    public static ValueTask<ImmutableDictionary<string, Weapon>> GetNameToWeaponMapAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        return metadataService.FromCacheAsDictionaryAsync<string, Weapon>(
            MetadataFileStrategies.Weapon,
            w => w.Name,
            token);
    }

    public static ValueTask<ImmutableDictionary<MonsterRelationshipId, Monster>> GetRelationshipIdToMonsterMapAsync(this IMetadataService metadataService, CancellationToken token = default)
    {
        return metadataService.FromCacheAsDictionaryAsync<MonsterRelationshipId, Monster>(
            MetadataFileStrategies.Monster,
            m => m.RelationshipId,
            token);
    }

    private static async ValueTask<ImmutableDictionary<TKey, TValue>> FromCacheAsDictionaryAsync<TKey, TValue>(this IMetadataService metadataService, MetadataFileStrategy strategy, Func<TValue, TKey> keySelector, CancellationToken token)
        where TKey : notnull
        where TValue : class
    {
        string keyName = TypeNameHelper.GetTypeDisplayName(typeof(TKey));
        string valueName = TypeNameHelper.GetTypeDisplayName(typeof(TValue));
        string cacheKey = $"{nameof(MetadataService)}.Cache.{strategy.Name}.Map.{keyName}.{valueName}";

        ImmutableDictionary<TKey, TValue>? result = await metadataService.MemoryCache.GetOrCreateAsync(cacheKey, async entry =>
        {
            ImmutableArray<TValue> array = await metadataService.FromCacheOrFileAsync<TValue>(strategy, token).ConfigureAwait(false);
            return array.ToImmutableDictionaryIgnoringDuplicateKeys(keySelector); // There are duplicate name items
        }).ConfigureAwait(false);

        ArgumentNullException.ThrowIfNull(result);
        return result;
    }

    private static async ValueTask<ImmutableDictionary<TKey, TValue>> FromCacheAsDictionaryAsync<TData, TKey, TValue>(this IMetadataService metadataService, MetadataFileStrategy strategy, Func<TData, TKey> keySelector, Func<TData, TValue> valueSelector, CancellationToken token)
        where TKey : notnull
        where TData : class
    {
        string keyName = TypeNameHelper.GetTypeDisplayName(typeof(TKey));
        string valueName = TypeNameHelper.GetTypeDisplayName(typeof(TValue));
        string cacheKey = $"{nameof(MetadataService)}.Cache.{strategy.Name}.Map.{keyName}.{valueName}";

        ImmutableDictionary<TKey, TValue>? result = await metadataService.MemoryCache.GetOrCreateAsync(cacheKey, async entry =>
        {
            ImmutableArray<TData> array = await metadataService.FromCacheOrFileAsync<TData>(strategy, token).ConfigureAwait(false);
            return array.ToImmutableDictionaryIgnoringDuplicateKeys(keySelector, valueSelector); // There are duplicate name items
        }).ConfigureAwait(false);

        ArgumentNullException.ThrowIfNull(result);
        return result;
    }

    private static ValueTask<ImmutableDictionary<TKey, TValue>> FromCacheAsDictionaryAsync<TData, TKey, TValue>(this IMetadataService metadataService, MetadataFileStrategy strategy, Func<ImmutableArray<TData>, IEnumerable<(TKey Key, TValue Value)>> transform, CancellationToken token)
        where TKey : notnull
        where TData : class
    {
        return FromCacheAsDictionaryAsync(metadataService, strategy, transform, kvp => kvp.Key, kvp => kvp.Value, token);
    }

    private static async ValueTask<ImmutableDictionary<TKey, TValue>> FromCacheAsDictionaryAsync<TData, TMiddle, TKey, TValue>(this IMetadataService metadataService, MetadataFileStrategy strategy, Func<ImmutableArray<TData>, IEnumerable<TMiddle>> transform, Func<TMiddle, TKey> keySelector, Func<TMiddle, TValue> valueSelector, CancellationToken token)
        where TKey : notnull
        where TData : class
    {
        string keyName = TypeNameHelper.GetTypeDisplayName(typeof(TKey));
        string middleName = TypeNameHelper.GetTypeDisplayName(typeof(TMiddle));
        string valueName = TypeNameHelper.GetTypeDisplayName(typeof(TValue));
        string cacheKey = $"{nameof(MetadataService)}.Cache.{strategy.Name}.Map.{keyName}.{middleName}.{valueName}";

        ImmutableDictionary<TKey, TValue>? result = await metadataService.MemoryCache.GetOrCreateAsync(cacheKey, async entry =>
        {
            ImmutableArray<TData> array = await metadataService.FromCacheOrFileAsync<TData>(strategy, token).ConfigureAwait(false);
            return transform(array).ToImmutableDictionaryIgnoringDuplicateKeys(keySelector, valueSelector); // There are duplicate name items
        }).ConfigureAwait(false);

        ArgumentNullException.ThrowIfNull(result);
        return result;
    }
}