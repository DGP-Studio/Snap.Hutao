// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.UI.Xaml.Data;
using Snap.Hutao.Web.Hoyolab.Takumi.Event.BbsSignReward;

namespace Snap.Hutao.ViewModel.Sign;

internal sealed partial class AwardView : IPropertyValuesProvider
{
    public required int Index { get; init; }

    public required string Icon { get; init; }

    public required string Name { get; init; }

    public required int Count { get; init; }

    public bool IsClaimed { get; set; }

    public static AwardView Create(Award award, int index)
    {
        return new()
        {
            Index = index + 1,
            Icon = award.Icon,
            Name = award.Name,
            Count = award.Count,
        };
    }
}