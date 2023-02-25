// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Metadata;
using Snap.Hutao.Model.Metadata.Achievement;
using Snap.Hutao.Model.Metadata.Avatar;
using Snap.Hutao.Model.Metadata.Item;
using Snap.Hutao.Model.Metadata.Monster;
using Snap.Hutao.Model.Metadata.Reliquary;
using Snap.Hutao.Model.Metadata.Weapon;

namespace Snap.Hutao.Service.Metadata;

/// <summary>
/// 接口实现部分
/// </summary>
internal sealed partial class MetadataService
{
    /// <inheritdoc/>
    public ValueTask<List<AchievementGoal>> GetAchievementGoalsAsync(CancellationToken token = default)
    {
        return FromCacheOrFileAsync<List<AchievementGoal>>("AchievementGoal", token);
    }

    /// <inheritdoc/>
    public ValueTask<List<Model.Metadata.Achievement.Achievement>> GetAchievementsAsync(CancellationToken token = default)
    {
        return FromCacheOrFileAsync<List<Model.Metadata.Achievement.Achievement>>("Achievement", token);
    }

    /// <inheritdoc/>
    public ValueTask<List<Avatar>> GetAvatarsAsync(CancellationToken token = default)
    {
        return FromCacheOrFileAsync<List<Avatar>>("Avatar", token);
    }

    /// <inheritdoc/>
    public ValueTask<List<Promote>> GetAvatarPromotesAsync(CancellationToken token = default)
    {
        return FromCacheOrFileAsync<List<Promote>>("AvatarPromote", token);
    }

    /// <inheritdoc/>
    public ValueTask<List<Promote>> GetWeaponPromotesAsync(CancellationToken token = default)
    {
        return FromCacheOrFileAsync<List<Promote>>("WeaponPromote", token);
    }

    /// <inheritdoc/>
    public ValueTask<List<GachaEvent>> GetGachaEventsAsync(CancellationToken token = default)
    {
        return FromCacheOrFileAsync<List<GachaEvent>>("GachaEvent", token);
    }

    /// <inheritdoc/>
    public ValueTask<List<Material>> GetMaterialsAsync(CancellationToken token = default)
    {
        return FromCacheOrFileAsync<List<Material>>("Material", token);
    }

    /// <inheritdoc/>
    public ValueTask<List<Monster>> GetMonstersAsync(CancellationToken token = default)
    {
        return FromCacheOrFileAsync<List<Monster>>("Monster", token);
    }

    /// <inheritdoc/>
    public ValueTask<List<Reliquary>> GetReliquariesAsync(CancellationToken token = default)
    {
        return FromCacheOrFileAsync<List<Reliquary>>("Reliquary", token);
    }

    /// <inheritdoc/>
    public ValueTask<List<ReliquaryAffix>> GetReliquaryAffixesAsync(CancellationToken token = default)
    {
        return FromCacheOrFileAsync<List<ReliquaryAffix>>("ReliquaryAffix", token);
    }

    /// <inheritdoc/>
    public ValueTask<List<ReliquaryLevel>> GetReliquaryLevelsAsync(CancellationToken token = default)
    {
        return FromCacheOrFileAsync<List<ReliquaryLevel>>("ReliquaryMainAffixLevel", token);
    }

    /// <inheritdoc/>
    public ValueTask<List<ReliquaryMainAffix>> GetReliquaryMainAffixesAsync(CancellationToken token = default)
    {
        return FromCacheOrFileAsync<List<ReliquaryMainAffix>>("ReliquaryMainAffix", token);
    }

    /// <inheritdoc/>
    public ValueTask<List<ReliquarySet>> GetReliquarySetsAsync(CancellationToken token = default)
    {
        return FromCacheOrFileAsync<List<ReliquarySet>>("ReliquarySet", token);
    }

    /// <inheritdoc/>
    public ValueTask<List<Weapon>> GetWeaponsAsync(CancellationToken token = default)
    {
        return FromCacheOrFileAsync<List<Weapon>>("Weapon", token);
    }
}
