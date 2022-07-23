// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI.UI.Animations;
using Microsoft.Graphics.Canvas.Effects;
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
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Snap.Hutao.Control.Image;

/// <summary>
/// 支持单色的图像
/// </summary>
public class MonoChrome : Microsoft.UI.Xaml.Controls.Control
{
    private static readonly DependencyProperty SourceProperty = Property<MonoChrome>.Depend(nameof(Source), string.Empty, OnSourceChanged);
    private static readonly ConcurrentCancellationTokenSource<MonoChrome> LoadingTokenSource = new();

    private SpriteVisual? spriteVisual;
    private CompositionColorBrush? backgroundBrush;

    /// <summary>
    /// 构造一个新的单色图像
    /// </summary>
    public MonoChrome()
    {
        SizeChanged += OnSizeChanged;
        ActualThemeChanged += OnActualThemeChanged;
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
        MonoChrome monoChrome = (MonoChrome)sender;
        string url = (string)arg.NewValue;

        ILogger<MonoChrome> logger = Ioc.Default.GetRequiredService<ILogger<MonoChrome>>();
        monoChrome.ApplyImageAsync(url, LoadingTokenSource.Register(monoChrome)).SafeForget(logger, OnApplyImageFailed);
    }

    private static void OnApplyImageFailed(Exception exception)
    {
        Ioc.Default
            .GetRequiredService<IInfoBarService>()
            .Error(exception, "应用单色背景时发生异常");
    }

    private static async Task<LoadedImageSurface> LoadImageSurfaceAsync(StorageFile storageFile, CancellationToken token)
    {
        using (IRandomAccessStream imageStream = await storageFile.OpenAsync(FileAccessMode.Read).AsTask(token))
        {
            BitmapDecoder decoder = await BitmapDecoder.CreateAsync(imageStream).AsTask(token);
            return LoadedImageSurface.StartLoadFromStream(imageStream);
        }
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

    private void OnActualThemeChanged(FrameworkElement sender, object args)
    {
        if (backgroundBrush != null)
        {
            SetBackgroundColor(backgroundBrush);
        }
    }

    private void UpdateVisual(SpriteVisual spriteVisual)
    {
        if (spriteVisual is not null)
        {
            spriteVisual.Size = ActualSize;
        }
    }

    private void SetBackgroundColor(CompositionColorBrush backgroundBrush)
    {
        ApplicationTheme theme = ActualTheme switch
        {
            ElementTheme.Light => ApplicationTheme.Light,
            ElementTheme.Dark => ApplicationTheme.Dark,
            _ => App.Current.RequestedTheme,
        };

        backgroundBrush.Color = theme switch
        {
            ApplicationTheme.Light => Colors.Black,
            ApplicationTheme.Dark => Colors.White,
            _ => throw Must.NeverHappen(),
        };
    }

    private async Task ApplyImageAsync(string url, CancellationToken token)
    {
        await AnimationBuilder.Create().Opacity(0d).StartAsync(this, token);

        StorageFile? storageFile = Must.NotNull((await GetCachedFileAsync(url))!);

        Compositor compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;

        LoadedImageSurface imageSurface = await LoadImageSurfaceAsync(storageFile, token);

        CompositionColorBrush blackLayerBursh = compositor.CreateColorBrush(Colors.Black);
        CompositionSurfaceBrush imageSurfaceBrush = compositor.CompositeSurfaceBrush(imageSurface, stretch: CompositionStretch.Uniform, vRatio: 0f);

        CompositionEffectBrush overlayBrush = compositor.CompositeBlendEffectBrush(blackLayerBursh, imageSurfaceBrush, BlendEffectMode.Overlay);
        CompositionEffectBrush opacityBrush = compositor.CompositeLuminanceToAlphaEffectBrush(overlayBrush);

        backgroundBrush = compositor.CreateColorBrush();
        SetBackgroundColor(backgroundBrush);
        CompositionEffectBrush alphaMaskEffectBrush = compositor.CompositeAlphaMaskEffectBrush(backgroundBrush, opacityBrush);

        spriteVisual = compositor.CompositeSpriteVisual(alphaMaskEffectBrush);
        UpdateVisual(spriteVisual);

        ElementCompositionPreview.SetElementChildVisual(this, spriteVisual);

        await AnimationBuilder.Create().Opacity(1d).StartAsync(this, token);
    }
}