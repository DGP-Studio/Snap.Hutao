// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI;
using CommunityToolkit.WinUI.Animations;
using Microsoft.UI.Composition;
using System.Numerics;

namespace Snap.Hutao.Control.Animation;

/// <summary>
/// 图片缩小动画
/// </summary>
[HighQuality]
internal sealed class ImageZoomOutAnimation : ImplicitAnimation<string, Vector3>
{
    /// <summary>
    /// 构造一个新的图片缩小动画
    /// </summary>
    public ImageZoomOutAnimation()
    {
        Duration = AnimationDurations.ImageZoom;
        EasingMode = Microsoft.UI.Xaml.Media.Animation.EasingMode.EaseOut;
        EasingType = CommunityToolkit.WinUI.Animations.EasingType.Circle;
        To = Core.StringLiterals.One;
    }

    /// <inheritdoc/>
    protected override string ExplicitTarget
    {
        get => nameof(Visual.Scale);
    }

    /// <inheritdoc/>
    protected override (Vector3?, Vector3?) GetParsedValues()
    {
        return (To?.ToVector3(), From?.ToVector3());
    }
}