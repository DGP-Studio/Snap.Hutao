// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;
using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.SpiralAbyss;

namespace Snap.Hutao.Web.Hutao.SpiralAbyss.Post;

internal sealed class SimpleRank
{
    private SimpleRank(SpiralAbyssRank rank)
    {
        AvatarId = rank.AvatarId;
        Value = rank.Value;
    }

    public AvatarId AvatarId { get; set; }

    public int Value { get; set; }

    public static SimpleRank? FromRank(SpiralAbyssRank? rank)
    {
        return rank is null ? null : new SimpleRank(rank);
    }
}
