// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Metadata;
using Snap.Hutao.Service.Metadata;
using Snap.Hutao.Service.Metadata.ContextAbstraction;
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
    private readonly ITaskContext taskContext;

    /// <inheritdoc/>
    public async ValueTask<Summary> CreateAsync(IEnumerable<Model.Entity.AvatarInfo> avatarInfos, CancellationToken token)
    {
        SummaryFactoryMetadataContext context = await metadataService
            .GetContextAsync<SummaryFactoryMetadataContext>(token)
            .ConfigureAwait(false);

        IOrderedEnumerable<AvatarView> avatars = avatarInfos
            .Where(a => !AvatarIds.IsPlayer(a.Info.AvatarId))
            .Select(a => SummaryAvatarFactory.Create(context, a))
            .OrderByDescending(a => a.Quality)
            .ThenByDescending(a => a.LevelNumber)
            .ThenBy(a => a.Element)
            .ThenBy(a => a.Weapon?.WeaponType)
            .ThenByDescending(a => a.FetterLevel);

        IList<AvatarView> views = [.. avatars];

        await taskContext.SwitchToMainThreadAsync();

        return new()
        {
            Avatars = new(views, true),
        };
    }
}