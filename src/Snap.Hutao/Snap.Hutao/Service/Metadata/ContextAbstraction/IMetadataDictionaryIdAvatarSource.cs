// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Service.Metadata.ContextAbstraction;

internal interface IMetadataDictionaryIdAvatarSource
{
    public Dictionary<AvatarId, Model.Metadata.Avatar.Avatar> IdAvatarMap { get; set; }
}