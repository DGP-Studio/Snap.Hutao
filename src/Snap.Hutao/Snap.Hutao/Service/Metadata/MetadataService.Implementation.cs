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
        return FromCacheOrFileAsync<List<AchievementGoal>>(FileNameAchievementGoal, token);
    }

    /// <inheritdoc/>
    public ValueTask<List<Model.Metadata.Achievement.Achievement>> GetAchievementsAsync(CancellationToken token = default)
    {
        return FromCacheOrFileAsync<List<Model.Metadata.Achievement.Achievement>>(FileNameAchievement, token);
    }

    /// <inheritdoc/>
    public ValueTask<List<Avatar>> GetAvatarsAsync(CancellationToken token = default)
    {
        return FromCacheOrFileAsync<List<Avatar>>(FileNameAvatar, token);
    }

    /// <inheritdoc/>
    public ValueTask<List<Promote>> GetAvatarPromotesAsync(CancellationToken token = default)
    {
        return FromCacheOrFileAsync<List<Promote>>(FileNameAvatarPromote, token);
    }

    /// <inheritdoc/>
    public ValueTask<List<GachaEvent>> GetGachaEventsAsync(CancellationToken token = default)
    {
        return FromCacheOrFileAsync<List<GachaEvent>>(FileNameGachaEvent, token);
    }

    /// <inheritdoc/>
    public ValueTask<List<Material>> GetMaterialsAsync(CancellationToken token = default)
    {
        return FromCacheOrFileAsync<List<Material>>(FileNameMaterial, token);
    }

    /// <inheritdoc/>
    public ValueTask<List<Monster>> GetMonstersAsync(CancellationToken token = default)
    {
        return FromCacheOrFileAsync<List<Monster>>(FileNameMonster, token);
    }

    /// <inheritdoc/>
    public ValueTask<List<Reliquary>> GetReliquariesAsync(CancellationToken token = default)
    {
        return FromCacheOrFileAsync<List<Reliquary>>(FileNameReliquary, token);
    }

    /// <inheritdoc/>
    public ValueTask<List<ReliquaryMainAffixLevel>> GetReliquaryLevelsAsync(CancellationToken token = default)
    {
        return FromCacheOrFileAsync<List<ReliquaryMainAffixLevel>>(FileNameReliquaryMainAffixLevel, token);
    }

    /// <inheritdoc/>
    public ValueTask<List<ReliquaryMainAffix>> GetReliquaryMainAffixesAsync(CancellationToken token = default)
    {
        return FromCacheOrFileAsync<List<ReliquaryMainAffix>>(FileNameReliquaryMainAffix, token);
    }

    /// <inheritdoc/>
    public ValueTask<List<ReliquarySet>> GetReliquarySetsAsync(CancellationToken token = default)
    {
        return FromCacheOrFileAsync<List<ReliquarySet>>(FileNameReliquarySet, token);
    }

    /// <inheritdoc/>
    public ValueTask<List<ReliquarySubAffix>> GetReliquarySubAffixesAsync(CancellationToken token = default)
    {
        return FromCacheOrFileAsync<List<ReliquarySubAffix>>(FileNameReliquarySubAffix, token);
    }

    /// <inheritdoc/>
    public ValueTask<List<Weapon>> GetWeaponsAsync(CancellationToken token = default)
    {
        return FromCacheOrFileAsync<List<Weapon>>(FileNameWeapon, token);
    }

    /// <inheritdoc/>
    public ValueTask<List<Promote>> GetWeaponPromotesAsync(CancellationToken token = default)
    {
        return FromCacheOrFileAsync<List<Promote>>(FileNameWeaponPromote, token);
    }
}
