// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI.UI.Animations;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Hosting;
using Microsoft.UI.Xaml.Media;
using Snap.Hutao.Core.Caching;
using Snap.Hutao.Service.Abstraction;
using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices;

namespace Snap.Hutao.Control.Image;

/// <summary>
/// 合成图像控件
/// 为其他图像类控件提供基类
/// </summary>
[HighQuality]
internal abstract class CompositionImage : Microsoft.UI.Xaml.Controls.Control
{
    private static readonly DependencyProperty SourceProperty = Property<CompositionImage>.Depend(nameof(Source), default(Uri), OnSourceChanged);
    private static readonly DependencyProperty EnableLazyLoadingProperty = Property<CompositionImage>.DependBoxed<bool>(nameof(EnableLazyLoading), BoxedValues.True);
    private static readonly ConcurrentCancellationTokenSource<CompositionImage> LoadingTokenSource = new();

    private readonly IServiceProvider serviceProvider;

    private SpriteVisual? spriteVisual;
    private bool isShow = true;

    /// <summary>
    /// 构造一个新的单色图像
    /// </summary>
    public CompositionImage()
    {
        serviceProvider = Ioc.Default;

        AllowFocusOnInteraction = false;
        IsDoubleTapEnabled = false;
        IsHitTestVisible = false;
        IsHoldingEnabled = false;
        IsRightTapEnabled = false;
        IsTabStop = false;

        SizeChanged += OnSizeChanged;
    }

    /// <summary>
    /// 源
    /// </summary>
    public Uri Source
    {
        get => (Uri)GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    /// <summary>
    /// 启用延迟加载
    /// </summary>
    public bool EnableLazyLoading
    {
        get => (bool)GetValue(EnableLazyLoadingProperty);
        set => SetValue(EnableLazyLoadingProperty, value);
    }

    /// <summary>
    /// 合成组合视觉
    /// </summary>
    /// <param name="compositor">合成器</param>
    /// <param name="imageSurface">图像表面</param>
    /// <returns>拼合视觉</returns>
    protected abstract SpriteVisual CompositeSpriteVisual(Compositor compositor, LoadedImageSurface imageSurface);

    /// <summary>
    /// 异步加载图像表面
    /// </summary>
    /// <param name="file">文件</param>
    /// <param name="token">取消令牌</param>
    /// <returns>加载的图像表面</returns>
    protected virtual async Task<LoadedImageSurface> LoadImageSurfaceAsync(string file, CancellationToken token)
    {
        TaskCompletionSource loadCompleteTaskSource = new();
        LoadedImageSurface surface = LoadedImageSurface.StartLoadFromUri(new(file));
        surface.LoadCompleted += (s, e) => loadCompleteTaskSource.TrySetResult();
        await loadCompleteTaskSource.Task.ConfigureAwait(true);
        return surface;
    }

    /// <summary>
    /// 更新视觉对象
    /// </summary>
    /// <param name="spriteVisual">拼合视觉</param>
    protected virtual void OnUpdateVisual(SpriteVisual spriteVisual)
    {
        spriteVisual.Size = ActualSize;
    }

    private static void OnSourceChanged(DependencyObject sender, DependencyPropertyChangedEventArgs arg)
    {
        CompositionImage image = (CompositionImage)sender;
        CancellationToken token = LoadingTokenSource.Register(image);
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
            infoBarService.Error(httpRequestException, string.Format(SH.ControlImageCompositionImageHttpRequest, uri));
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

    private async Task ApplyImageAsync(Uri? uri, CancellationToken token)
    {
        await HideAsync(token).ConfigureAwait(true);

        if (uri != null)
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

            if (imageSurface != null)
            {
                spriteVisual = CompositeSpriteVisual(compositor, imageSurface);
                OnUpdateVisual(spriteVisual);

                ElementCompositionPreview.SetElementChildVisual(this, spriteVisual);
                await ShowAsync(token).ConfigureAwait(true);
            }
        }
    }

    private async Task ShowAsync(CancellationToken token)
    {
        if (!isShow)
        {
            isShow = true;

            if (EnableLazyLoading)
            {
                await AnimationBuilder.Create().Opacity(from: 0D, to: 1D).StartAsync(this, token).ConfigureAwait(true);
            }
            else
            {
                Opacity = 1;
            }
        }
    }

    private async Task HideAsync(CancellationToken token)
    {
        if (isShow)
        {
            isShow = false;

            if (EnableLazyLoading)
            {
                await AnimationBuilder.Create().Opacity(from: 1D, to: 0D).StartAsync(this, token).ConfigureAwait(true);
            }
            else
            {
                Opacity = 0;
            }
        }
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (e.NewSize != e.PreviousSize && spriteVisual != null)
        {
            OnUpdateVisual(spriteVisual);
        }
    }
}