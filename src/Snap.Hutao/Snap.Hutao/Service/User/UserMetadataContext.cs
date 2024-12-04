// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Metadata.Avatar;
using System.Collections.Immutable;

namespace Snap.Hutao.Service.User;

internal class UserMetadataContext : IUserMetadataContext
{
    public ImmutableArray<ProfilePicture> ProfilePictures { get; set; } = default!;
}
