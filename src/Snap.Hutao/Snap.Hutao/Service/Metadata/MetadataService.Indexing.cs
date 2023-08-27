// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Extensions.Caching.Memory;
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Metadata;
using Snap.Hutao.Model.Metadata.Avatar;
using Snap.Hutao.Model.Metadata.Item;
using Snap.Hutao.Model.Metadata.Reliquary;
using Snap.Hutao.Model.Metadata.Weapon;
using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Service.Metadata;

/// <summary>
/// 索引部分
/// </summary>
internal sealed partial class MetadataService
{
    /// <inheritdoc/>
    public ValueTask<Dictionary<EquipAffixId, ReliquarySet>> GetEquipAffixIdToReliquarySetMapAsync(CancellationToken token = default)
    {
        return FromCacheAsDictionaryAsync<EquipAffixId, ReliquarySet>(FileNameReliquarySet, r => r.EquipAffixId, token);
    }

    public async ValueTask<Dictionary<ExtendedEquipAffixId, ReliquarySet>> GetExtendedEquipAffixIdToReliquarySetMapAsync(CancellationToken token = default)
    {
        string cacheKey = $"{nameof(MetadataService)}.Cache.{FileNameReliquarySet}.Map.{nameof(ExtendedEquipAffixId)}";

        if (memoryCache.TryGetValue(cacheKey, out object? value))
        {
            ArgumentNullException.ThrowIfNull(value);
            return (Dictionary<ExtendedEquipAffixId, ReliquarySet>)value;
        }

        List<ReliquarySet> list = await FromCacheOrFileAsync<List<ReliquarySet>>(FileNameReliquarySet, token).ConfigureAwait(false);

        Dictionary<ExtendedEquipAffixId, ReliquarySet> dict = list
            .SelectMany(set => set.EquipAffixIds.Select(id => (id, set)))
            .ToDictionary(tuple => tuple.id, tuple => tuple.set);
        return memoryCache.Set(cacheKey, dict);
    }

    /// <inheritdoc/>
    public ValueTask<Dictionary<AchievementId, Model.Metadata.Achievement.Achievement>> GetIdToAchievementMapAsync(CancellationToken token = default)
    {
        return FromCacheAsDictionaryAsync<AchievementId, Model.Metadata.Achievement.Achievement>(FileNameAchievement, a => a.Id, token);
    }

    /// <inheritdoc/>
    public ValueTask<Dictionary<AvatarId, Avatar>> GetIdToAvatarMapAsync(CancellationToken token = default)
    {
        return FromCacheAsDictionaryAsync<AvatarId, Avatar>(FileNameAvatar, a => a.Id, token);
    }

    /// <inheritdoc/>
    public async ValueTask<Dictionary<MaterialId, DisplayItem>> GetIdToDisplayItemAndMaterialMapAsync(CancellationToken token = default)
    {
        string cacheKey = $"{nameof(MetadataService)}.Cache.{nameof(DisplayItem)}.Map.{typeof(MaterialId).Name}";

        if (memoryCache.TryGetValue(cacheKey, out object? value))
        {
            ArgumentNullException.ThrowIfNull(value);
            return (Dictionary<MaterialId, DisplayItem>)value;
        }

        Dictionary<MaterialId, DisplayItem> displays = await FromCacheAsDictionaryAsync<MaterialId, DisplayItem>(FileNameDisplayItem, a => a.Id, token).ConfigureAwait(false);
        Dictionary<MaterialId, Material> materials = await GetIdToMaterialMapAsync(token).ConfigureAwait(false);

        // TODO: Cache this
        Dictionary<MaterialId, DisplayItem> results = new(displays);

        foreach ((MaterialId id, DisplayItem material) in materials)
        {
            results[id] = material;
        }

        return memoryCache.Set(cacheKey, results);
    }

    /// <inheritdoc/>
    public ValueTask<Dictionary<MaterialId, Material>> GetIdToMaterialMapAsync(CancellationToken token = default)
    {
        return FromCacheAsDictionaryAsync<MaterialId, Material>(FileNameMaterial, a => a.Id, token);
    }

    /// <inheritdoc/>
    public ValueTask<Dictionary<AvatarId, ReliquaryAffixWeight>> GetIdToReliquaryAffixWeightMapAsync(CancellationToken token = default)
    {
        return FromCacheAsDictionaryAsync<AvatarId, ReliquaryAffixWeight>(FileNameReliquaryAffixWeight, r => r.AvatarId, token);
    }

    /// <inheritdoc/>
    public ValueTask<Dictionary<ReliquarySubAffixId, ReliquarySubAffix>> GetIdToReliquarySubAffixMapAsync(CancellationToken token = default)
    {
        return FromCacheAsDictionaryAsync<ReliquarySubAffixId, ReliquarySubAffix>(FileNameReliquarySubAffix, a => a.Id, token);
    }

    /// <inheritdoc/>
    public ValueTask<Dictionary<ReliquaryMainAffixId, FightProperty>> GetIdToReliquaryMainPropertyMapAsync(CancellationToken token = default)
    {
        return FromCacheAsDictionaryAsync<ReliquaryMainAffixId, FightProperty, ReliquaryMainAffix>(FileNameReliquaryMainAffix, r => r.Id, r => r.Type, token);
    }

    /// <inheritdoc/>
    public ValueTask<Dictionary<WeaponId, Weapon>> GetIdToWeaponMapAsync(CancellationToken token = default)
    {
        return FromCacheAsDictionaryAsync<WeaponId, Weapon>(FileNameWeapon, w => w.Id, token);
    }

    /// <inheritdoc/>
    public ValueTask<Dictionary<Level, Dictionary<GrowCurveType, float>>> GetLevelToAvatarCurveMapAsync(CancellationToken token = default)
    {
        return FromCacheAsDictionaryAsync<Level, Dictionary<GrowCurveType, float>, GrowCurve>(FileNameAvatarCurve, a => a.Level, a => a.Map, token);
    }

    /// <inheritdoc/>
    public ValueTask<Dictionary<Level, Dictionary<GrowCurveType, float>>> GetLevelToMonsterCurveMapAsync(CancellationToken token = default)
    {
        return FromCacheAsDictionaryAsync<Level, Dictionary<GrowCurveType, float>, GrowCurve>(FileNameMonsterCurve, m => m.Level, m => m.Map, token);
    }

    /// <inheritdoc/>
    public ValueTask<Dictionary<Level, Dictionary<GrowCurveType, float>>> GetLevelToWeaponCurveMapAsync(CancellationToken token = default)
    {
        return FromCacheAsDictionaryAsync<Level, Dictionary<GrowCurveType, float>, GrowCurve>(FileNameWeaponCurve, w => w.Level, w => w.Map, token);
    }

    /// <inheritdoc/>
    public ValueTask<Dictionary<string, Avatar>> GetNameToAvatarMapAsync(CancellationToken token = default)
    {
        return FromCacheAsDictionaryAsync<string, Avatar>(FileNameAvatar, a => a.Name, token);
    }

    /// <inheritdoc/>
    public ValueTask<Dictionary<string, Weapon>> GetNameToWeaponMapAsync(CancellationToken token = default)
    {
        return FromCacheAsDictionaryAsync<string, Weapon>(FileNameWeapon, w => w.Name, token);
    }
}