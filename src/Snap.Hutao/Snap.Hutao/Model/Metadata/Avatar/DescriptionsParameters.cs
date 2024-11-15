// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;
using System.Collections.Immutable;

namespace Snap.Hutao.Model.Metadata.Avatar;

internal sealed class DescriptionsParameters
{
    public required ImmutableArray<string> Descriptions { get; init; }

    public required LevelParametersCollection<SkillLevel, float> Parameters { get; init; }
}