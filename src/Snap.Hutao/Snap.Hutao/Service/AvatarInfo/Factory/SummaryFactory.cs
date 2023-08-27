// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Metadata;
using Snap.Hutao.Service.Metadata;
using Snap.Hutao.ViewModel.AvatarProperty;

namespace Snap.Hutao.Service.AvatarInfo.Factory;

/// <summary>
/// 简述工厂
/// </summary>
[HighQuality]
[ConstructorGenerated]
[Injection(InjectAs.Transient, typeof(ISummaryFactory))]
internal sealed partial class SummaryFactory : ISummaryFactory
{
    private readonly IMetadataService metadataService;

    /// <inheritdoc/>
    public async ValueTask<Summary> CreateAsync(IEnumerable<Model.Entity.AvatarInfo> avatarInfos, CancellationToken token)
    {
        SummaryMetadataContext metadataContext = new()
        {
            IdAvatarMap = await metadataService.GetIdToAvatarMapAsync(token).ConfigureAwait(false),
            IdWeaponMap = await metadataService.GetIdToWeaponMapAsync(token).ConfigureAwait(false),
            IdReliquaryAffixWeightMap = await metadataService.GetIdToReliquaryAffixWeightMapAsync(token).ConfigureAwait(false),
            IdReliquaryMainAffixMap = await metadataService.GetIdToReliquaryMainPropertyMapAsync(token).ConfigureAwait(false),
            IdReliquarySubAffixMap = await metadataService.GetIdToReliquarySubAffixMapAsync(token).ConfigureAwait(false),
            ReliquaryLevels = await metadataService.GetReliquaryLevelsAsync(token).ConfigureAwait(false),
            Reliquaries = await metadataService.GetReliquariesAsync(token).ConfigureAwait(false),
        };

        return new()
        {
            Avatars = avatarInfos
                .Where(a => !AvatarIds.IsPlayer(a.Info.AvatarId))
                .Select(a => new SummaryAvatarFactory(metadataContext, a).Create())
                .OrderByDescending(a => a.LevelNumber)
                .ThenBy(a => a.Name)
                .ToList(),

            // .ThenByDescending(a => a.Quality)
            // .ThenByDescending(a => a.ActivatedConstellationCount)
        };
    }
}