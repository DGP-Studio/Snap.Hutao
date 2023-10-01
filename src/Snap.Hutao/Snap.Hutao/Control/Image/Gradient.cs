// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;

namespace Snap.Hutao.Control.Image;

/// <summary>
/// 渐变图像
/// </summary>
[HighQuality]
[DependencyProperty("BackgroundDirection", typeof(GradientDirection), GradientDirection.TopToBottom)]
[DependencyProperty("ForegroundDirection", typeof(GradientDirection), GradientDirection.TopToBottom)]
internal sealed partial class Gradient : CompositionImage
{
    private double imageAspectRatio;

    /// <inheritdoc/>
    protected override void UpdateVisual(SpriteVisual? spriteVisual)
    {
        if (spriteVisual is null)
        {
            return;
        }

        Height = Math.Clamp(ActualWidth / imageAspectRatio, 0D, MaxHeight);
        spriteVisual.Size = ActualSize;
    }

    protected override void LoadImageSurfaceCompleted(LoadedImageSurface surface)
    {
        imageAspectRatio = surface.NaturalSize.AspectRatio();
    }

    /// <inheritdoc/>
    protected override SpriteVisual CompositeSpriteVisual(Compositor compositor, LoadedImageSurface imageSurface)
    {
        CompositionSurfaceBrush imageSurfaceBrush = compositor.CompositeSurfaceBrush(imageSurface, stretch: CompositionStretch.UniformToFill, vRatio: 0f);

        CompositionLinearGradientBrush backgroundBrush = compositor.CompositeLinearGradientBrush(BackgroundDirection, new(0, Colors.White), new(1, Colors.Black));
        CompositionLinearGradientBrush foregroundBrush = compositor.CompositeLinearGradientBrush(ForegroundDirection, new(0, Colors.White), new(1, Colors.Black));

        CompositionEffectBrush gradientEffectBrush = compositor.CompositeBlendEffectBrush(backgroundBrush, foregroundBrush);
        CompositionEffectBrush opacityMaskEffectBrush = compositor.CompositeLuminanceToAlphaEffectBrush(gradientEffectBrush);
        CompositionEffectBrush alphaMaskEffectBrush = compositor.CompositeAlphaMaskEffectBrush(imageSurfaceBrush, opacityMaskEffectBrush);

        return compositor.CompositeSpriteVisual(alphaMaskEffectBrush);
    }
}