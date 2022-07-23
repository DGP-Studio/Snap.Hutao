// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI.UI.Animations;
using Microsoft.UI;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Hosting;
using Microsoft.UI.Xaml.Media;
using Snap.Hutao.Core;
using Snap.Hutao.Core.Caching;
using Snap.Hutao.Core.Threading;
using Snap.Hutao.Extension;
using Snap.Hutao.Service.Abstraction;
using System.Numerics;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Snap.Hutao.Control.Image;

/// <summary>
/// 支持渐变的图像
/// </summary>
public class Gradient : Microsoft.UI.Xaml.Controls.Control
{
    private static readonly DependencyProperty SourceProperty = Property<Gradient>.Depend(nameof(Source), string.Empty, OnSourceChanged);
    private static readonly ConcurrentCancellationTokenSource<Gradient> LoadingTokenSource = new();

    private SpriteVisual? spriteVisual;
    private double imageAspectRatio;

    /// <summary>
    /// 构造一个新的渐变图像
    /// </summary>
    public Gradient()
    {
        SizeChanged += OnSizeChanged;
    }

    /// <summary>
    /// 源
    /// </summary>
    public string Source
    {
        get => (string)GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    private static void OnSourceChanged(DependencyObject sender, DependencyPropertyChangedEventArgs arg)
    {
        Gradient gradient = (Gradient)sender;
        string url = (string)arg.NewValue;

        ILogger<Gradient> logger = Ioc.Default.GetRequiredService<ILogger<Gradient>>();
        gradient.ApplyImageAsync(url, LoadingTokenSource.Register(gradient)).SafeForget(logger, OnApplyImageFailed);
    }

    private static void OnApplyImageFailed(Exception exception)
    {
        Ioc.Default
            .GetRequiredService<IInfoBarService>()
            .Error(exception, "应用渐变图像时发生异常");
    }

    private static Task<StorageFile?> GetCachedFileAsync(string url)
    {
        return Ioc.Default.GetRequiredService<IImageCache>().GetFileFromCacheAsync(new(url));
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (e.NewSize != e.PreviousSize && spriteVisual is not null)
        {
            UpdateVisual(spriteVisual);
        }
    }

    private void UpdateVisual(SpriteVisual spriteVisual)
    {
        if (spriteVisual is not null)
        {
            Height = (double)Math.Clamp(ActualWidth / imageAspectRatio, 0, MaxHeight);
            spriteVisual.Size = ActualSize;
        }
    }

    private async Task ApplyImageAsync(string url, CancellationToken token)
    {
        await AnimationBuilder.Create().Opacity(0d).StartAsync(this, token);

        StorageFile? storageFile = Must.NotNull((await GetCachedFileAsync(url))!);

        Compositor compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;

        LoadedImageSurface imageSurface = await LoadImageSurfaceAsync(storageFile, token);

        CompositionSurfaceBrush imageSurfaceBrush = compositor.CompositeSurfaceBrush(imageSurface, stretch: CompositionStretch.UniformToFill, vRatio: 0f);

        CompositionLinearGradientBrush backgroundBrush = compositor.CompositeLinearGradientBrush(new(1f, 0), Vector2.UnitY, new(0, Colors.White), new(1, Colors.Black));
        CompositionLinearGradientBrush foregroundBrush = compositor.CompositeLinearGradientBrush(Vector2.Zero, Vector2.UnitY, new(0, Colors.White), new(0.95f, Colors.Black));

        CompositionEffectBrush gradientEffectBrush = compositor.CompositeBlendEffectBrush(backgroundBrush, foregroundBrush);
        CompositionEffectBrush opacityMaskEffectBrush = compositor.CompositeLuminanceToAlphaEffectBrush(gradientEffectBrush);
        CompositionEffectBrush alphaMaskEffectBrush = compositor.CompositeAlphaMaskEffectBrush(imageSurfaceBrush, opacityMaskEffectBrush);

        spriteVisual = compositor.CompositeSpriteVisual(alphaMaskEffectBrush);
        UpdateVisual(spriteVisual);

        ElementCompositionPreview.SetElementChildVisual(this, spriteVisual);

        await AnimationBuilder.Create().Opacity(1d).StartAsync(this, token);
    }

    private async Task<LoadedImageSurface> LoadImageSurfaceAsync(StorageFile storageFile, CancellationToken token)
    {
        using (IRandomAccessStream imageStream = await storageFile.OpenAsync(FileAccessMode.Read).AsTask(token))
        {
            BitmapDecoder decoder = await BitmapDecoder.CreateAsync(imageStream).AsTask(token);
            imageAspectRatio = decoder.PixelWidth / (double)decoder.PixelHeight;

            return LoadedImageSurface.StartLoadFromStream(imageStream);
        }
    }
}