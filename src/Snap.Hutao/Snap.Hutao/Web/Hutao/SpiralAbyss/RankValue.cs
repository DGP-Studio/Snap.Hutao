// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hutao.SpiralAbyss;

internal sealed class RankValue : ItemRate<int, double>
{
    [JsonConstructor]
    public RankValue(int item, int value, double rate, double rateOnAvatar)
        : base(item, rate)
    {
        Value = value;
        RateOnAvatar = rateOnAvatar;
    }

    public int Value { get; set; }

    public double RateOnAvatar { get; set; }
}