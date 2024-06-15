// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Metadata.Avatar;

namespace Snap.Hutao.Service.User;

internal class UserMetadataContext : IUserMetadataContext
{
    public List<ProfilePicture> ProfilePictures { get; set; } = default!;
}
