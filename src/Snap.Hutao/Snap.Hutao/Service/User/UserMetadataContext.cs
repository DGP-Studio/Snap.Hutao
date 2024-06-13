// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Metadata.Avatar;
using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Service.User;

internal class UserMetadataContext : IUserMetadataContext
{
    public Dictionary<ProfilePictureId, ProfilePicture> IdProfilePictureMap { get; set; } = default!;

    public Dictionary<AvatarId, ProfilePicture> AvatarIdProfilePictureMap { get; set; } = default!;

    public Dictionary<CostumeId, ProfilePicture> CostumeIdProfilePictureMap { get; set; } = default!;
}
