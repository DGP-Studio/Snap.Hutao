// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI.Animations;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Hosting;
using Microsoft.UI.Xaml.Media;
using Snap.Hutao.Core.Caching;
using Snap.Hutao.Core.Graphics;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.UI.Xaml.Media.Animation;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices;

namespace Snap.Hutao.UI.Xaml.Control.Image;

[DependencyProperty("EnableShowHideAnimation", typeof(bool), true)]
[DependencyProperty("Source", typeof(Uri), default!, nameof(OnSourceChanged))]
internal abstract partial class CompositionImage : Microsoft.UI.Xaml.Controls.Control
{
    private readonly ConcurrentCancellationTokenSource loadingTokenSource = new();

    private readonly IServiceProvider serviceProvider;

    private TaskCompletionSource? surfaceLoadTaskCompletionSource;
    private SpriteVisual? spriteVisual;
    private bool isShow = true;

    protected CompositionImage()
    {
        serviceProvider = this.ServiceProvider();
        this.DisableInteraction();

        SizeChanged += OnSizeChanged;
    }

    protected abstract SpriteVisual CompositeSpriteVisual(Compositor compositor, LoadedImageSurface imageSurface);

    protected virtual void LoadImageSurfaceCompleted(LoadedImageSurface surface)
    {
    }

    protected virtual void UpdateVisual(SpriteVisual spriteVisual)
    {
        spriteVisual.Size = ActualSize;
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        if (!string.IsNullOrEmpty(Source.OriginalString))
        {
            OnSourceChangedCore(Source);
        }
    }

    private static void OnSourceChanged(DependencyObject sender, DependencyPropertyChangedEventArgs arg)
    {
        CompositionImage image = (CompositionImage)sender;
        CancellationToken token = image.loadingTokenSource.Register();
        IServiceProvider serviceProvider = image.serviceProvider;
        ILogger<CompositionImage> logger = serviceProvider.GetRequiredService<ILogger<CompositionImage>>();

        // source is valid
        if (arg.NewValue is Uri inner)
        {
            if (!string.IsNullOrEmpty(inner.OriginalString))
            {
                // value is different from old one
                if (inner != (arg.OldValue as Uri))
                {
                    image.OnSourceChangedCore(inner);
                }
            }
            else
            {
                image.HideAsync(token).SafeForget(logger);
            }
        }
    }

    private static void OnApplyImageFailed(IServiceProvider serviceProvider, Uri? uri, Exception exception)
    {
        Debugger.Break();
        IInfoBarService infoBarService = serviceProvider.GetRequiredService<IInfoBarService>();

        if (exception is HttpRequestException httpRequestException)
        {
            infoBarService.Error(httpRequestException, SH.FormatControlImageCompositionImageHttpRequest(uri));
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

    private void OnSourceChangedCore(Uri? uri)
    {
        CancellationToken token = loadingTokenSource.Register();
        ILogger<CompositionImage> logger = serviceProvider.GetRequiredService<ILogger<CompositionImage>>();
        ApplyImageAsync(uri, token).SafeForget(logger, ex => OnApplyImageFailed(serviceProvider, uri, ex));
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
            catch (COMException ex)
            {
                _ = ex;
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
            else
            {
                Debugger.Break();
            }
        }
    }

    private async ValueTask<LoadedImageSurface> LoadImageSurfaceAsync(string file, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        surfaceLoadTaskCompletionSource = new();
        LoadedImageSurface? surface = default;
        try
        {
            surface = LoadedImageSurface.StartLoadFromUri(file.ToUri());
            surface.LoadCompleted += OnLoadImageSurfaceLoadCompleted;
            if (surface.DecodedPhysicalSize.Size() <= 0D)
            {
                try
                {
                    await surfaceLoadTaskCompletionSource.Task.WithCancellation(token).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                }
            }

            LoadImageSurfaceCompleted(surface);
            return surface;
        }
        finally
        {
            if (surface is not null)
            {
                surface.LoadCompleted -= OnLoadImageSurfaceLoadCompleted;
            }
        }
    }

    private async ValueTask ShowAsync(CancellationToken token)
    {
        if (!isShow)
        {
            isShow = true;

            if (EnableShowHideAnimation)
            {
                await AnimationBuilder
                    .Create()
                    .Opacity(from: 0D, to: 1D, duration: Constants.ImageScaleFadeIn)
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

            if (EnableShowHideAnimation)
            {
                await AnimationBuilder
                    .Create()
                    .Opacity(from: 1D, to: 0D, duration: Constants.ImageScaleFadeOut)
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
}