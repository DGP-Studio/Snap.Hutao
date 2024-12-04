// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Model.Metadata.Achievement;

internal sealed class AchievementGoal
{
    public required AchievementGoalId Id { get; init; }

    public required uint Order { get; init; }

    public required string Name { get; init; }

    public Reward? FinishReward { get; init; }

    public required string Icon { get; init; }
}