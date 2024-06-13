// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Service.Metadata.ContextAbstraction;

internal interface IMetadataDictionaryAvatarIdProfilePictureSource
{
    public Dictionary<AvatarId, Model.Metadata.Avatar.ProfilePicture> AvatarIdProfilePictureMap { get; set; }
}
