// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;

namespace Snap.Hutao.UI.Xaml.Media.Animation;

internal static class Constants
{
    /// <summary>
    /// 1
    /// </summary>
    public const string One = "1";

    /// <summary>
    /// 1.1
    /// </summary>
    public const string OnePointOne = "1.1";

    /// <summary>
    /// 图片缩放动画
    /// </summary>
    public static readonly TimeSpan ImageZoom = TimeSpan.FromSeconds(0.5);

    /// <summary>
    /// 图像淡入
    /// </summary>
    public static readonly TimeSpan ImageScaleFadeIn = TimeSpan.FromSeconds(0.3);

    /// <summary>
    /// 图像淡出
    /// </summary>
    public static readonly TimeSpan ImageScaleFadeOut = TimeSpan.FromSeconds(0.2);

    public static readonly TimeSpan ImageOpacityFadeInOut = TimeSpan.FromSeconds(1);

    public static readonly GridLength ZeroGridLength = new(0);
}