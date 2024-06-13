// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Service.Metadata.ContextAbstraction;

internal interface IMetadataDictionaryIdProfilePictureSource
{
    public Dictionary<ProfilePictureId, Model.Metadata.Avatar.ProfilePicture> IdProfilePictureMap { get; set; }
}
