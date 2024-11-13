// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Model.Metadata;

internal sealed class RoleCombatSchedule
{
    public RoleCombatScheduleId Id { get; set; }

    public DateTimeOffset Begin { get; set; }

    public DateTimeOffset End { get; set; }

    public List<ElementType> Elements { get; set; } = default!;

    public List<AvatarId> SpecialAvatars { get; set; } = default!;

    public List<AvatarId> InitialAvatars { get; set; } = default!;
}