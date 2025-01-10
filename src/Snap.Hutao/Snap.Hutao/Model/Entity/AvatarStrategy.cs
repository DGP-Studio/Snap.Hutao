// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Snap.Hutao.Model.Entity;

[Table("avatar_strategies")]
internal sealed class AvatarStrategy
{
    [Key]
    public uint AvatarId { get; set; }

    public int ChineseStrategyId { get; set; }

    public int OverseaStrategyId { get; set; }

    [NotMapped]
    public Uri ChineseStrategyUrl { get => $"https://bbs.mihoyo.com/ys/strategy/channel/map/39/{ChineseStrategyId}?bbs_presentation_style=no_header".ToUri(); }

    [NotMapped]
    public Uri OverseaStrategyUrl { get => $"https://www.hoyolab.com/guidelist?game_id=2&guide_id={OverseaStrategyId}".ToUri(); }
}