// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Metadata.Avatar;
using Snap.Hutao.ViewModel.Complex;

namespace Snap.Hutao.Model.Binding.Hutao;

/// <summary>
/// 角色命座信息
/// </summary>
[HighQuality]
internal sealed class AvatarConstellationInfoView : AvatarView
{
    public AvatarConstellationInfoView(Avatar avatar, double rate, double? lastRate)
        : base(avatar, rate, lastRate)
    {
    }

    public List<RateAndDelta> Rates { get; set; } = default!;
}