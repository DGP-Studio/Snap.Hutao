// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate;

/// <summary>
/// 分组可计算值
/// </summary>
[HighQuality]
internal abstract class GroupCalculable : Calculable
{
    /// <summary>
    /// 组Id
    /// </summary>
    [JsonPropertyName("group_id")]
    public int GroupId { get; set; }

    /// <inheritdoc/>
    public override PromotionDelta ToPromotionDelta()
    {
        return new()
        {
            LevelCurrent = LevelCurrent,
            LevelTarget = LevelTarget,
            Id = GroupId,
        };
    }
}
