// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Model.Metadata.Achievement;

internal sealed class AchievementGoal
{
    public AchievementGoalId Id { get; set; }

    public uint Order { get; set; }

    public string Name { get; set; } = default!;

    public Reward? FinishReward { get; set; }

    public string Icon { get; set; } = default!;
}