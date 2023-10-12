// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI.Animations;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Hosting;
using Microsoft.UI.Xaml.Media;
using Snap.Hutao.Control.Animation;
using Snap.Hutao.Control.Extension;
using Snap.Hutao.Core.Caching;
using Snap.Hutao.Service.Notification;
using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices;
using Windows.Foundation;

namespace Snap.Hutao.Control.Image;

/// <summary>
/// 合成图像控件
/// 为其他图像类控件提供基类
/// </summary>
[HighQuality]
[DependencyProperty("EnableLazyLoading", typeof(bool), true, nameof(OnSourceChanged))]
[DependencyProperty("Source", typeof(Uri), default!, nameof(OnSourceChanged))]
internal abstract partial class CompositionImage : Microsoft.UI.Xaml.Controls.Control
{
    private readonly ConcurrentCancellationTokenSource loadingTokenSource = new();

    private readonly IServiceProvider serviceProvider;

    private readonly RoutedEventHandler unloadEventHandler;
    private readonly SizeChangedEventHandler sizeChangedEventHandler;
    private readonly TypedEventHandler<LoadedImageSurface, LoadedImageSourceLoadCompletedEventArgs> loadedImageSourceLoadCompletedEventHandler;

    private TaskCompletionSource? surfaceLoadTaskCompletionSource;
    private SpriteVisual? spriteVisual;
    private bool isShow = true;

    /// <summary>
    /// 构造一个新的单色图像
    /// </summary>
    protected CompositionImage()
    {
        serviceProvider = this.ServiceProvider();
        this.DisableInteraction();

        unloadEventHandler = OnUnload;
        Unloaded += unloadEventHandler;

        sizeChangedEventHandler = OnSizeChanged;
        SizeChanged += sizeChangedEventHandler;

        loadedImageSourceLoadCompletedEventHandler = OnLoadImageSurfaceLoadCompleted;
    }

    /// <summary>
    /// 合成组合视觉
    /// </summary>
    /// <param name="compositor">合成器</param>
    /// <param name="imageSurface">图像表面</param>
    /// <returns>拼合视觉</returns>
    protected abstract SpriteVisual CompositeSpriteVisual(Compositor compositor, LoadedImageSurface imageSurface);

    protected virtual void LoadImageSurfaceCompleted(LoadedImageSurface surface)
    {
    }

    protected virtual void Unloading()
    {
    }

    /// <summary>
    /// 更新视觉对象
    /// </summary>
    /// <param name="spriteVisual">拼合视觉</param>
    protected virtual void UpdateVisual(SpriteVisual spriteVisual)
    {
        spriteVisual.Size = ActualSize;
    }

    private static void OnSourceChanged(DependencyObject sender, DependencyPropertyChangedEventArgs arg)
    {
        CompositionImage image = (CompositionImage)sender;
        CancellationToken token = image.loadingTokenSource.Register();
        IServiceProvider serviceProvider = image.serviceProvider;
        ILogger<CompositionImage> logger = serviceProvider.GetRequiredService<ILogger<CompositionImage>>();

        // source is valid
        if (arg.NewValue is Uri inner && !string.IsNullOrEmpty(inner.OriginalString))
        {
            // value is different from old one
            if (inner != (arg.OldValue as Uri))
            {
                image
                    .ApplyImageAsync(inner, token)
                    .SafeForget(logger, ex => OnApplyImageFailed(serviceProvider, inner, ex));
            }
        }
        else
        {
            image.HideAsync(token).SafeForget(logger);
        }
    }

    private static void OnApplyImageFailed(IServiceProvider serviceProvider, Uri? uri, Exception exception)
    {
        IInfoBarService infoBarService = serviceProvider.GetRequiredService<IInfoBarService>();

        if (exception is HttpRequestException httpRequestException)
        {
            infoBarService.Error(httpRequestException, SH.ControlImageCompositionImageHttpRequest.Format(uri));
        }
        else
        {
            Exception baseException = exception.GetBaseException();
            if (baseException is not COMException)
            {
                infoBarService.Error(baseException, SH.ControlImageCompositionImageSystemException);
            }
        }
    }

    private async ValueTask ApplyImageAsync(Uri? uri, CancellationToken token)
    {
        await HideAsync(token).ConfigureAwait(true);

        if (uri is not null)
        {
            LoadedImageSurface? imageSurface = null;
            Compositor compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;

            IImageCache imageCache = serviceProvider.GetRequiredService<IImageCache>();
            string file = await imageCache.GetFileFromCacheAsync(uri).ConfigureAwait(true);

            try
            {
                imageSurface = await LoadImageSurfaceAsync(file, token).ConfigureAwait(true);
            }
            catch (COMException)
            {
                imageCache.Remove(uri);
            }
            catch (IOException)
            {
                imageCache.Remove(uri);
            }

            if (imageSurface is not null)
            {
                using (imageSurface)
                {
                    spriteVisual = CompositeSpriteVisual(compositor, imageSurface);
                    UpdateVisual(spriteVisual);

                    ElementCompositionPreview.SetElementChildVisual(this, spriteVisual);
                    await ShowAsync(token).ConfigureAwait(true);
                }
            }
        }
    }

    private async ValueTask<LoadedImageSurface> LoadImageSurfaceAsync(string file, CancellationToken token)
    {
        surfaceLoadTaskCompletionSource = new();
        LoadedImageSurface? surface = default;
        try
        {
            surface = LoadedImageSurface.StartLoadFromUri(file.ToUri());
            surface.LoadCompleted += loadedImageSourceLoadCompletedEventHandler;
            if (surface.NaturalSize.Size() <= 0D)
            {
                await surfaceLoadTaskCompletionSource.Task.ConfigureAwait(true);
            }

            LoadImageSurfaceCompleted(surface);
            return surface;
        }
        finally
        {
            if (surface is not null)
            {
                surface.LoadCompleted -= loadedImageSourceLoadCompletedEventHandler;
            }
        }
    }

    private async ValueTask ShowAsync(CancellationToken token)
    {
        if (!isShow)
        {
            isShow = true;

            if (EnableLazyLoading)
            {
                await AnimationBuilder
                    .Create()
                    .Opacity(from: 0D, to: 1D, duration: AnimationDurations.ImageFadeIn)
                    .StartAsync(this, token)
                    .ConfigureAwait(true);
            }
            else
            {
                Opacity = 1;
            }
        }
    }

    private async ValueTask HideAsync(CancellationToken token)
    {
        if (isShow)
        {
            isShow = false;

            if (EnableLazyLoading)
            {
                await AnimationBuilder
                    .Create()
                    .Opacity(from: 1D, to: 0D, duration: AnimationDurations.ImageFadeOut)
                    .StartAsync(this, token)
                    .ConfigureAwait(true);
            }
            else
            {
                Opacity = 0;
            }
        }
    }

    private void OnLoadImageSurfaceLoadCompleted(LoadedImageSurface surface, LoadedImageSourceLoadCompletedEventArgs e)
    {
        surfaceLoadTaskCompletionSource?.TrySetResult();
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (e.NewSize != e.PreviousSize && spriteVisual is not null)
        {
            UpdateVisual(spriteVisual);
        }
    }

    private void OnUnload(object sender, RoutedEventArgs e)
    {
        Unloading();
        spriteVisual?.Dispose();
        spriteVisual = null;

        SizeChanged -= sizeChangedEventHandler;
        Unloaded -= unloadEventHandler;
    }
}