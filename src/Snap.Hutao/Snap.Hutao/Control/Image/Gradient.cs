// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml.Media;
using System.Numerics;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Snap.Hutao.Control.Image;

/// <summary>
/// 支持渐变的图像
/// </summary>
public class Gradient : CompositionImage
{
    private double imageAspectRatio;

    /// <inheritdoc/>
    protected override void OnUpdateVisual(SpriteVisual spriteVisual)
    {
        if (spriteVisual is not null)
        {
            Height = (double)Math.Clamp(ActualWidth / imageAspectRatio, 0, MaxHeight);
            spriteVisual.Size = ActualSize;
        }
    }

    /// <inheritdoc/>
    protected override async Task<LoadedImageSurface> LoadImageSurfaceAsync(StorageFile storageFile, CancellationToken token)
    {
        using (IRandomAccessStream imageStream = await storageFile.OpenAsync(FileAccessMode.Read).AsTask(token))
        {
            BitmapDecoder decoder = await BitmapDecoder.CreateAsync(imageStream).AsTask(token);
            imageAspectRatio = decoder.PixelWidth / (double)decoder.PixelHeight;

            return LoadedImageSurface.StartLoadFromStream(imageStream);
        }
    }

    /// <inheritdoc/>
    protected override SpriteVisual CompositeSpriteVisual(Compositor compositor, LoadedImageSurface imageSurface)
    {
        CompositionSurfaceBrush imageSurfaceBrush = compositor.CompositeSurfaceBrush(imageSurface, stretch: CompositionStretch.UniformToFill, vRatio: 0f);

        CompositionLinearGradientBrush backgroundBrush = compositor.CompositeLinearGradientBrush(new(1f, 0), Vector2.UnitY, new(0, Colors.White), new(1, Colors.Black));
        CompositionLinearGradientBrush foregroundBrush = compositor.CompositeLinearGradientBrush(Vector2.Zero, Vector2.UnitY, new(0, Colors.White), new(0.95f, Colors.Black));

        CompositionEffectBrush gradientEffectBrush = compositor.CompositeBlendEffectBrush(backgroundBrush, foregroundBrush);
        CompositionEffectBrush opacityMaskEffectBrush = compositor.CompositeLuminanceToAlphaEffectBrush(gradientEffectBrush);
        CompositionEffectBrush alphaMaskEffectBrush = compositor.CompositeAlphaMaskEffectBrush(imageSurfaceBrush, opacityMaskEffectBrush);

        return compositor.CompositeSpriteVisual(alphaMaskEffectBrush);
    }
}