// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Metadata.Avatar;
using System.Collections.Immutable;

namespace Snap.Hutao.Service.Metadata.ContextAbstraction.ImmutableArray;

internal interface IMetadataArrayAvatarSource
{
    ImmutableArray<Avatar> Avatars { get; set; }
}