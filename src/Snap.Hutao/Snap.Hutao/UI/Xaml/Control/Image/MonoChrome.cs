// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Graphics.Canvas.Effects;
using Microsoft.UI;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Snap.Hutao.UI.Composition;
using Snap.Hutao.UI.Xaml.Control.Theme;
using Windows.Foundation;

namespace Snap.Hutao.UI.Xaml.Control.Image;

internal sealed class MonoChrome : CompositionImage
{
    private CompositionColorBrush? backgroundBrush;

    public MonoChrome()
    {
        ActualThemeChanged += OnActualThemeChanged;
    }

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

    private void OnActualThemeChanged(FrameworkElement sender, object args)
    {
        if (backgroundBrush is not null)
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