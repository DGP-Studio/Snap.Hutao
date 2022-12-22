// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Metadata;
using Snap.Hutao.Model.Metadata.Avatar;
using Snap.Hutao.Model.Metadata.Reliquary;
using Snap.Hutao.Model.Metadata.Weapon;
using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Service.Metadata;

/// <summary>
/// 索引部分
/// </summary>
internal partial class MetadataService
{
    /// <inheritdoc/>
    public ValueTask<Dictionary<EquipAffixId, ReliquarySet>> GetEquipAffixIdToReliquarySetMapAsync(CancellationToken token = default)
    {
        return FromCacheAsDictionaryAsync<EquipAffixId, ReliquarySet>("ReliquarySet", r => r.EquipAffixId, token);
    }

    /// <inheritdoc/>
    public ValueTask<Dictionary<AvatarId, Avatar>> GetIdToAvatarMapAsync(CancellationToken token = default)
    {
        return FromCacheAsDictionaryAsync<AvatarId, Avatar>("Avatar", a => a.Id, token);
    }

    /// <inheritdoc/>
    public ValueTask<Dictionary<MaterialId, Material>> GetIdToMaterialMapAsync(CancellationToken token = default)
    {
        return FromCacheAsDictionaryAsync<MaterialId, Material>("Material", a => a.Id, token);
    }

    /// <inheritdoc/>
    public ValueTask<Dictionary<ReliquaryAffixId, ReliquaryAffix>> GetIdReliquaryAffixMapAsync(CancellationToken token = default)
    {
        return FromCacheAsDictionaryAsync<ReliquaryAffixId, ReliquaryAffix>("ReliquaryAffix", a => a.Id, token);
    }

    /// <inheritdoc/>
    public ValueTask<Dictionary<ReliquaryMainAffixId, FightProperty>> GetIdToReliquaryMainPropertyMapAsync(CancellationToken token = default)
    {
        return FromCacheAsDictionaryAsync<ReliquaryMainAffixId, FightProperty, ReliquaryAffixBase>("ReliquaryMainAffix", r => r.Id, r => r.Type, token);
    }

    /// <inheritdoc/>
    public ValueTask<Dictionary<WeaponId, Weapon>> GetIdToWeaponMapAsync(CancellationToken token = default)
    {
        return FromCacheAsDictionaryAsync<WeaponId, Weapon>("Weapon", w => w.Id, token);
    }

    /// <inheritdoc/>
    public ValueTask<Dictionary<string, Avatar>> GetNameToAvatarMapAsync(CancellationToken token = default)
    {
        return FromCacheAsDictionaryAsync<string, Avatar>("Avatar", a => a.Name, token);
    }

    /// <inheritdoc/>
    public ValueTask<Dictionary<string, Weapon>> GetNameToWeaponMapAsync(CancellationToken token = default)
    {
        return FromCacheAsDictionaryAsync<string, Weapon>("Weapon", w => w.Name, token);
    }
}