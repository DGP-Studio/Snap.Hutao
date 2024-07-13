// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Abstraction;
using Snap.Hutao.Model.Intrinsic;

namespace Snap.Hutao.Model.InterChange;

internal sealed class HutaoReservedAchievement : IMappingFrom<HutaoReservedAchievement, Model.Entity.Achievement>
{
    public required uint Id { get; set; }

    public required uint Current { get; set; }

    public required DateTimeOffset Time { get; set; }

    public required AchievementStatus Status { get; set; }

    public static HutaoReservedAchievement From(Entity.Achievement source)
    {
        return new()
        {
            Id = source.Id,
            Current = source.Current,
            Time = source.Time,
            Status = source.Status,
        };
    }
}