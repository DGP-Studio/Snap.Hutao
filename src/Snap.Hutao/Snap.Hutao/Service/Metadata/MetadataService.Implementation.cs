// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Metadata;
using Snap.Hutao.Model.Metadata.Achievement;
using Snap.Hutao.Model.Metadata.Avatar;
using Snap.Hutao.Model.Metadata.Reliquary;
using Snap.Hutao.Model.Metadata.Weapon;

namespace Snap.Hutao.Service.Metadata;

/// <summary>
/// 接口实现部分
/// </summary>
internal partial class MetadataService
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
    public ValueTask<List<ReliquaryAffixBase>> GetReliquaryMainAffixesAsync(CancellationToken token = default)
    {
        return FromCacheOrFileAsync<List<ReliquaryAffixBase>>("ReliquaryMainAffix", token);
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
