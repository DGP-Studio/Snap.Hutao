// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Graphics.Canvas.Effects;
using Microsoft.UI;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;

namespace Snap.Hutao.Control.Image;

/// <summary>
/// 支持单色的图像
/// </summary>
public class MonoChrome : CompositionImage
{
    private CompositionColorBrush? backgroundBrush;

    /// <summary>
    /// 构造一个新的单色图像
    /// </summary>
    public MonoChrome()
    {
        ActualThemeChanged += OnActualThemeChanged;
    }

    /// <inheritdoc/>
    protected override SpriteVisual CompositeSpriteVisual(Compositor compositor, LoadedImageSurface imageSurface)
    {
        CompositionColorBrush blackLayerBursh = compositor.CreateColorBrush(Colors.Black);
        CompositionSurfaceBrush imageSurfaceBrush = compositor.CompositeSurfaceBrush(imageSurface, stretch: CompositionStretch.Uniform, vRatio: 0f);
        CompositionEffectBrush overlayBrush = compositor.CompositeBlendEffectBrush(blackLayerBursh, imageSurfaceBrush, BlendEffectMode.Overlay);
        CompositionEffectBrush opacityBrush = compositor.CompositeLuminanceToAlphaEffectBrush(overlayBrush);

        backgroundBrush = compositor.CreateColorBrush();
        SetBackgroundColor(backgroundBrush);
        CompositionEffectBrush alphaMaskEffectBrush = compositor.CompositeAlphaMaskEffectBrush(backgroundBrush, opacityBrush);

        return compositor.CompositeSpriteVisual(alphaMaskEffectBrush);
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
        ApplicationTheme theme = ActualTheme switch
        {
            ElementTheme.Light => ApplicationTheme.Light,
            ElementTheme.Dark => ApplicationTheme.Dark,
            _ => Ioc.Default.GetRequiredService<App>().RequestedTheme,
        };

        backgroundBrush.Color = theme switch
        {
            ApplicationTheme.Light => Colors.Black,
            ApplicationTheme.Dark => Colors.White,
            _ => throw Must.NeverHappen(),
        };
    }
}
