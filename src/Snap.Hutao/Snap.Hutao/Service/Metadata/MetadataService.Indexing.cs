// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Metadata.Avatar;
using Snap.Hutao.Model.Metadata.Reliquary;
using Snap.Hutao.Model.Metadata.Weapon;

namespace Snap.Hutao.Service.Metadata;

/// <summary>
/// 索引部分
/// </summary>
internal partial class MetadataService
{
    /// <inheritdoc/>
    public ValueTask<Dictionary<int, ReliquarySet>> GetEquipAffixIdToReliquarySetMapAsync(CancellationToken token = default)
    {
        return FromCacheAsDictionaryAsync<int, ReliquarySet>("ReliquarySet", r => r.EquipAffixId, token);
    }

    /// <inheritdoc/>
    public ValueTask<Dictionary<int, Avatar>> GetIdToAvatarMapAsync(CancellationToken token = default)
    {
        return FromCacheAsDictionaryAsync<int, Avatar>("Avatar", a => a.Id, token);
    }

    /// <inheritdoc/>
    public ValueTask<Dictionary<int, ReliquaryAffix>> GetIdReliquaryAffixMapAsync(CancellationToken token = default)
    {
        return FromCacheAsDictionaryAsync<int, ReliquaryAffix>("ReliquaryAffix", a => a.Id, token);
    }

    /// <inheritdoc/>
    public ValueTask<Dictionary<int, FightProperty>> GetIdToReliquaryMainPropertyMapAsync(CancellationToken token = default)
    {
        return FromCacheAsDictionaryAsync<int, FightProperty, ReliquaryAffixBase>("ReliquaryMainAffix", r => r.Id, r => r.Type, token);
    }

    /// <inheritdoc/>
    public ValueTask<Dictionary<int, Weapon>> GetIdToWeaponMapAsync(CancellationToken token = default)
    {
        return FromCacheAsDictionaryAsync<int, Weapon>("Weapon", w => w.Id, token);
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