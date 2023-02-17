// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Metadata.Avatar;

namespace Snap.Hutao.Model.Binding.Hutao;

/// <summary>
/// 角色命座信息
/// </summary>
[HighQuality]
internal sealed class ComplexAvatarConstellationInfo : ComplexAvatar
{
    /// <summary>
    /// 构造一个新的角色命座信息
    /// </summary>
    /// <param name="avatar">角色</param>
    /// <param name="rate">持有率</param>
    /// <param name="rates">命座比率</param>
    public ComplexAvatarConstellationInfo(Avatar avatar, double rate, IEnumerable<double> rates)
        : base(avatar, rate)
    {
        Rates = rates.Select(r => $"{r:P3}").ToList();
    }

    /// <summary>
    /// 命座比率
    /// </summary>
    public List<string> Rates { get; set; }
}
