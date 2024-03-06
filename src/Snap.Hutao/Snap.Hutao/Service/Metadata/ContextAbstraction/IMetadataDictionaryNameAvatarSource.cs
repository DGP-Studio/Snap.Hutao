// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Metadata.ContextAbstraction;

internal interface IMetadataDictionaryNameAvatarSource
{
    public Dictionary<string, Model.Metadata.Avatar.Avatar> NameAvatarMap { get; set; }
}