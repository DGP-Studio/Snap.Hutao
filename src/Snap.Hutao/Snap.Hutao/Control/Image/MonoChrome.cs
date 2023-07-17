// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Graphics.Canvas.Effects;
using Microsoft.UI;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Snap.Hutao.Control.Theme;
using Windows.Foundation;

namespace Snap.Hutao.Control.Image;

/// <summary>
/// 支持单色的图像
/// </summary>
[HighQuality]
internal sealed class MonoChrome : CompositionImage
{
    private readonly TypedEventHandler<FrameworkElement, object> actualThemeChangedEventHandler;

    private CompositionColorBrush? backgroundBrush;

    /// <summary>
    /// 构造一个新的单色图像
    /// </summary>
    public MonoChrome()
    {
        actualThemeChangedEventHandler = OnActualThemeChanged;
        ActualThemeChanged += actualThemeChangedEventHandler;
    }

    /// <inheritdoc/>
    protected override SpriteVisual CompositeSpriteVisual(Compositor compositor, LoadedImageSurface imageSurface)
    {
        CompositionColorBrush blackLayerBrush = compositor.CreateColorBrush(Colors.Black);
        CompositionSurfaceBrush imageSurfaceBrush = compositor.CompositeSurfaceBrush(imageSurface, stretch: CompositionStretch.Uniform, vRatio: 0f);
        CompositionEffectBrush overlayBrush = compositor.CompositeBlendEffectBrush(blackLayerBrush, imageSurfaceBrush, BlendEffectMode.Overlay);
        CompositionEffectBrush opacityBrush = compositor.CompositeLuminanceToAlphaEffectBrush(overlayBrush);

        backgroundBrush = compositor.CreateColorBrush();
        SetBackgroundColor(backgroundBrush);
        CompositionEffectBrush alphaMaskEffectBrush = compositor.CompositeAlphaMaskEffectBrush(backgroundBrush, opacityBrush);

        return compositor.CompositeSpriteVisual(alphaMaskEffectBrush);
    }

    protected override void Unloading()
    {
        ActualThemeChanged -= actualThemeChangedEventHandler;

        backgroundBrush?.Dispose();
        backgroundBrush = null;

        base.Unloading();
    }

    private void OnActualThemeChanged(FrameworkElement sender, object args)
    {
        if (backgroundBrush != null)
        {
            SetBackgroundColor(backgroundBrush);
        }
    }

    private void SetBackgroundColor(CompositionColorBrush backgroundBrush)
    {
        ApplicationTheme theme = ThemeHelper.ElementToApplication(ActualTheme);

        backgroundBrush.Color = theme switch
        {
            ApplicationTheme.Light => Colors.Black,
            ApplicationTheme.Dark => Colors.White,
            _ => Colors.Transparent,
        };
    }
}