// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.HardChallenge;

internal class HardChallengeBlingAvatar : HardChallengeSimpleAvatar
{
    /// <summary>
    /// 最终是否上榜
    /// </summary>
    [JsonPropertyName("is_plus")]
    public required bool IsPlus { get; init; }

    public Uri? SideIcon { get; init; }
}