// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Metadata.Avatar;
using Snap.Hutao.UI.Xaml.Data;
using Snap.Hutao.ViewModel.AvatarProperty;

namespace Snap.Hutao.Service.AvatarInfo.Factory;

[GeneratedConstructor]
[Service(ServiceLifetime.Transient, typeof(ISummaryFactory))]
internal sealed partial class SummaryFactory : ISummaryFactory
{
    private readonly ITaskContext taskContext;

    public async ValueTask<Summary> CreateAsync(SummaryFactoryMetadataContext context, IEnumerable<Model.Entity.AvatarInfo> avatarInfos, CancellationToken token)
    {
        await taskContext.SwitchToBackgroundAsync();

        IOrderedEnumerable<AvatarView> avatars = avatarInfos
            .Where(a => a.Info2 is not null && !AvatarIds.IsPlayer(a.Info2.Base.Id))
            .Select(a => SummaryAvatarFactory.Create(context, a))
            .OrderByDescending(a => a.Quality)
            .ThenByDescending(a => a.LevelNumber)
            .ThenBy(a => a.Element)
            .ThenBy(a => a.Weapon?.WeaponType)
            .ThenByDescending(a => a.FetterLevel);

        return Summary.Create(avatars.AsAdvancedCollectionView());
    }
}