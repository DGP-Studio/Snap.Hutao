﻿// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.UI.Xaml.Control.Image;

/// <summary>
/// 渐变锚点
/// </summary>
[HighQuality]
internal readonly struct GradientStop
{
    /// <summary>
    /// 偏移
    /// </summary>
    public readonly float Offset;

    /// <summary>
    /// 颜色
    /// </summary>
    public readonly Windows.UI.Color Color;

    /// <summary>
    /// 构造一个新的渐变锚点
    /// </summary>
    /// <param name="offset">偏移</param>
    /// <param name="color">颜色</param>
    public GradientStop(float offset, Windows.UI.Color color)
    {
        Offset = offset;
        Color = color;
    }
}