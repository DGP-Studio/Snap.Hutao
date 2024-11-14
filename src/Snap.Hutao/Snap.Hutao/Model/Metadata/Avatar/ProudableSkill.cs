// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Model.Metadata.Avatar;

internal sealed partial class ProudableSkill : Skill
{
    public SkillGroupId GroupId { get; set; }

    public DescriptionsParameters Proud { get; set; } = default!;
}