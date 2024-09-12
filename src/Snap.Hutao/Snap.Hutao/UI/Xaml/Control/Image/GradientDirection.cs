// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.UI.Xaml.Control.Image;

/// <summary>
/// 渐变方向
/// </summary>
[HighQuality]
internal enum GradientDirection
{
    /// <summary>
    /// 下到上
    /// </summary>
    BottomToTop,

    /// <summary>
    /// 左下到右上
    /// </summary>
    LeftBottomToRightTop,

    /// <summary>
    /// 左到右
    /// </summary>
    LeftToRight,

    /// <summary>
    /// 左上到右下
    /// </summary>
    LeftTopToRightBottom,

    /// <summary>
    /// 右下到左上
    /// </summary>
    RightBottomToLeftTop,

    /// <summary>
    /// 右到左
    /// </summary>
    RightToLeft,

    /// <summary>
    /// 右上到左下
    /// </summary>
    RightTopToLeftBottom,

    /// <summary>
    /// 上到下
    /// </summary>
    TopToBottom,
}