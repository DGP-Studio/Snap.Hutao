// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Service.Metadata.ContextAbstraction;

internal interface IMetadataDictionaryCostumeIdProfilePictureSource
{
    public Dictionary<CostumeId, Model.Metadata.Avatar.ProfilePicture> CostumeIdProfilePictureMap { get; set; }
}
