// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Metadata.Avatar;

namespace Snap.Hutao.Service.Metadata.ContextAbstraction;

internal interface IMetadataListProfilePictureSource
{
    public List<ProfilePicture> ProfilePictures { get; set; }
}
