// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;
using System.Collections.Immutable;

namespace Snap.Hutao.Model.Metadata.Avatar;

internal sealed class DescriptionsParameters
{
    public ImmutableArray<string> Descriptions { get; set; } = default!;

    public LevelParametersCollection<SkillLevel, float> Parameters { get; set; } = default!;
}