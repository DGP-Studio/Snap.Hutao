// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Metadata;

namespace Snap.Hutao.Service.Metadata.ContextAbstraction;

internal interface IMetadataListChapterSource
{
    List<Chapter> Chapters { get; set; }
}
