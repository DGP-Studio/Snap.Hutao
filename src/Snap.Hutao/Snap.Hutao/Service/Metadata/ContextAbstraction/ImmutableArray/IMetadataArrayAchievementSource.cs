// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Immutable;

namespace Snap.Hutao.Service.Metadata.ContextAbstraction.ImmutableArray;

internal interface IMetadataArrayAchievementSource
{
    ImmutableArray<Model.Metadata.Achievement.Achievement> Achievements { get; set; }
}