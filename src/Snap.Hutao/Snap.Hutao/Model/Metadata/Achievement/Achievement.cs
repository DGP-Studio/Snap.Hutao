// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Model.Metadata.Achievement;

internal sealed class Achievement : IDefaultIdentity<AchievementId>
{
    public required AchievementId Id { get; init; }

    public required AchievementGoalId Goal { get; init; }

    public required uint Order { get; init; }

    public AchievementId PreviousId { get; init; }

    public required string Title { get; init; }

    public required string Description { get; init; }

    public required Reward FinishReward { get; init; }

    public bool IsDeleteWatcherAfterFinish { get; init; }

    public required uint Progress { get; init; }

    public string? Icon { get; init; }

    public required string Version { get; init; }

    public bool IsDailyQuest { get; init; }
}