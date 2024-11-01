// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Model.Metadata.Achievement;

internal sealed class Achievement
{
    public AchievementId Id { get; set; }

    public AchievementGoalId Goal { get; set; }

    public uint Order { get; set; }

    public AchievementId PreviousId { get; set; }

    public string Title { get; set; } = default!;

    public string Description { get; set; } = default!;

    public Reward FinishReward { get; set; } = default!;

    public bool IsDeleteWatcherAfterFinish { get; set; }

    public uint Progress { get; set; }

    public string? Icon { get; set; }

    public string Version { get; set; } = default!;

    public bool IsDailyQuest { get; set; }
}