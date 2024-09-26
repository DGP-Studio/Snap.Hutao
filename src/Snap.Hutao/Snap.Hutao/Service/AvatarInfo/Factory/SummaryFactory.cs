// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Metadata.Avatar;
using Snap.Hutao.Service.Metadata;
using Snap.Hutao.Service.Metadata.ContextAbstraction;
using Snap.Hutao.UI.Xaml.Data;
using Snap.Hutao.ViewModel.AvatarProperty;

namespace Snap.Hutao.Service.AvatarInfo.Factory;

[ConstructorGenerated]
[Injection(InjectAs.Transient, typeof(ISummaryFactory))]
internal sealed partial class SummaryFactory : ISummaryFactory
{
    private readonly IMetadataService metadataService;

    public async ValueTask<Summary> CreateAsync(IEnumerable<Model.Entity.AvatarInfo> avatarInfos, CancellationToken token)
    {
        SummaryFactoryMetadataContext context = await metadataService
            .GetContextAsync<SummaryFactoryMetadataContext>(token)
            .ConfigureAwait(false);

        IOrderedEnumerable<AvatarView> avatars = avatarInfos
            .Where(a => a.Info2 is not null && !AvatarIds.IsPlayer(a.Info2.Base.Id))
            .Select(a => SummaryAvatarFactory.Create(context, a))
            .OrderByDescending(a => a.Quality)
            .ThenByDescending(a => a.LevelNumber)
            .ThenBy(a => a.Element)
            .ThenBy(a => a.Weapon?.WeaponType)
            .ThenByDescending(a => a.FetterLevel);

        IList<AvatarView> views = [.. avatars];

        return new()
        {
            Avatars = views.ToAdvancedCollectionView(),
        };
    }
}