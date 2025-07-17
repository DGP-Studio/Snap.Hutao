// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;
using System.Collections.Immutable;

namespace Snap.Hutao.Service.Metadata.ContextAbstraction.ImmutableDictionary;

internal interface IMetadataDictionaryIdAvatarSource
{
    ImmutableDictionary<AvatarId, Model.Metadata.Avatar.Avatar> IdAvatarMap { get; set; }
}