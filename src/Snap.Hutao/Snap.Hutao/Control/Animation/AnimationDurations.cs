// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Control.Animation;

/// <summary>
/// 动画时长
/// </summary>
[HighQuality]
internal static class AnimationDurations
{
    /// <summary>
    /// 图片缩放动画
    /// </summary>
    public static readonly TimeSpan ImageZoom = TimeSpan.FromSeconds(0.5);

    /// <summary>
    /// 图像淡入
    /// </summary>
    public static readonly TimeSpan ImageFadeIn = TimeSpan.FromSeconds(0.3);

    /// <summary>
    /// 图像淡出
    /// </summary>
    public static readonly TimeSpan ImageFadeOut = TimeSpan.FromSeconds(0.2);
}