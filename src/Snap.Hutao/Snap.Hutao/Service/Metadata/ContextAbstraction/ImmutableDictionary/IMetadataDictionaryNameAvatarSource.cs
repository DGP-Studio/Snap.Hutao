// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Immutable;

namespace Snap.Hutao.Service.Metadata.ContextAbstraction.ImmutableDictionary;

internal interface IMetadataDictionaryNameAvatarSource
{
    ImmutableDictionary<string, Model.Metadata.Avatar.Avatar> NameAvatarMap { get; set; }
}