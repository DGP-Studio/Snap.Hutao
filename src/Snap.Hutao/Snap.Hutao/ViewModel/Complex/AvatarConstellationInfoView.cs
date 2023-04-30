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
    /// <summary>
    /// 构造一个新的角色命座信息
    /// </summary>
    /// <param name="avatar">角色</param>
    /// <param name="rate">持有率</param>
    /// <param name="rates">命座比率</param>
    public AvatarConstellationInfoView(Avatar avatar, double rate, List<double> rates)
        : base(avatar, rate)
    {
        Rates = rates.SelectList(r => $"{r:P3}");
    }

    /// <summary>
    /// 命座比率
    /// </summary>
    public List<string> Rates { get; }
}
