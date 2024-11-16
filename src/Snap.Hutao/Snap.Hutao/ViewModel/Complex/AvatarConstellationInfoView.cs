// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Metadata.Avatar;

namespace Snap.Hutao.ViewModel.Complex;

internal sealed class AvatarConstellationInfoView : AvatarView
{
    public AvatarConstellationInfoView(Avatar avatar, double rate, double? lastRate)
        : base(avatar, rate, lastRate)
    {
    }

    public List<RateAndDelta> Rates { get; set; } = default!;
}