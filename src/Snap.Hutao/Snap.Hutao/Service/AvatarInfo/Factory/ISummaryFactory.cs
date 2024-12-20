// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.ViewModel.AvatarProperty;

namespace Snap.Hutao.Service.AvatarInfo.Factory;

internal interface ISummaryFactory
{
    ValueTask<Summary> CreateAsync(SummaryFactoryMetadataContext context, IEnumerable<Model.Entity.AvatarInfo> avatarInfos, CancellationToken token);
}