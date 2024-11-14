// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Model.Metadata.Avatar;

internal sealed class DescriptionsParameters
{
    public List<string> Descriptions { get; set; } = default!;

    public List<LevelParameters<SkillLevel, float>> Parameters { get; set; } = default!;
}
