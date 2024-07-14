// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;

namespace Snap.Hutao.ViewModel.AvatarProperty;

internal class ReliquarySubProperty
{
    public ReliquarySubProperty(FightProperty type, string value, float score)
    {
        Name = type.GetLocalizedDescription();
        Value = value;
        Score = score;

        // only 0.25 | 0.50 | 0.75 | 1.00
        Opacity = score == 0 ? 0.25 : Math.Ceiling(score / 25) / 4;
    }

    public string Name { get; }

    public string Value { get; }

    public double Opacity { get; }

    internal float Score { get; }
}