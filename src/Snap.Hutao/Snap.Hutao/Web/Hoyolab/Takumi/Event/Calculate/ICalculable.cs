// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;

namespace Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate;

/// <summary>
/// 可计算源
/// </summary>
public interface ICalculable
{
    /// <summary>
    /// 名称
    /// </summary>
    string Name { get; }

    /// <summary>
    /// 图标
    /// </summary>
    Uri Icon { get; }

    /// <summary>
    /// 星级
    /// </summary>
    ItemQuality Quality { get; }

    /// <summary>
    /// 当前等级
    /// </summary>
    int LevelCurrent { get; set; }

    /// <summary>
    /// 目标等级
    /// </summary>
    int LevelTarget { get; set; }
}