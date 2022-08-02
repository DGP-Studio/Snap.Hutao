// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI.UI.Animations;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Hosting;
using Microsoft.UI.Xaml.Media;
using Snap.Hutao.Core;
using Snap.Hutao.Core.Caching;
using Snap.Hutao.Core.Exception;
using Snap.Hutao.Core.Threading;
using Snap.Hutao.Extension;
using Snap.Hutao.Service.Abstraction;
using System.Net.Http;
using System.Runtime.InteropServices;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Snap.Hutao.Control.Image;

/// <summary>
/// 合成图像控件
/// 为其他图像类控件提供基类
/// </summary>
public abstract class CompositionImage : Microsoft.UI.Xaml.Controls.Control
{
    private static readonly DependencyProperty SourceProperty = Property<CompositionImage>.Depend(nameof(Source), default(Uri), OnSourceChanged);
    private static readonly ConcurrentCancellationTokenSource<CompositionImage> LoadingTokenSource = new();

    private readonly IImageCache imageCache;

    private SpriteVisual? spriteVisual;

    /// <summary>
    /// 构造一个新的单色图像
    /// </summary>
    public CompositionImage()
    {
        imageCache = Ioc.Default.GetRequiredService<IImageCache>();
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
    /// 合成组合视觉
    /// </summary>
    /// <param name="compositor">合成器</param>
    /// <param name="imageSurface">图像表面</param>
    /// <returns>拼合视觉</returns>
    protected abstract SpriteVisual CompositeSpriteVisual(Compositor compositor, LoadedImageSurface imageSurface);

    /// <summary>
    /// 异步加载图像表面
    /// </summary>
    /// <param name="storageFile">文件</param>
    /// <param name="token">取消令牌</param>
    /// <returns>加载的图像表面</returns>
    protected virtual async Task<LoadedImageSurface> LoadImageSurfaceAsync(StorageFile storageFile, CancellationToken token)
    {
        using (IRandomAccessStream imageStream = await storageFile.OpenAsync(FileAccessMode.Read).AsTask(token))
        {
            return LoadedImageSurface.StartLoadFromStream(imageStream);
        }
    }

    /// <summary>
    /// 更新视觉对象
    /// </summary>
    /// <param name="spriteVisual">拼合视觉</param>
    protected virtual void OnUpdateVisual(SpriteVisual spriteVisual)
    {
        spriteVisual.Size = ActualSize;
    }

    private static void OnApplyImageFailed(Uri? uri, Exception exception)
    {
        IInfoBarService infoBarService = Ioc.Default.GetRequiredService<IInfoBarService>();

        if (exception is HttpRequestException httpRequestException)
        {
            infoBarService.Warning($"GET {uri}\n{httpRequestException}");
        }
        else
        {
            infoBarService.Error(exception, $"应用 {nameof(CompositionImage)} 的源时发生异常");
        }
    }

    private static void OnSourceChanged(DependencyObject sender, DependencyPropertyChangedEventArgs arg)
    {
        CompositionImage image = (CompositionImage)sender;
        CancellationToken token = LoadingTokenSource.Register(image);
        ILogger<CompositionImage> logger = Ioc.Default.GetRequiredService<ILogger<CompositionImage>>();

        // source is valid
        if (arg.NewValue is Uri inner && !string.IsNullOrEmpty(inner.Host))
        {
            // value is different from old one
            if (inner != (arg.OldValue as Uri))
            {
                image.ApplyImageInternalAsync(inner, token).SafeForget(logger, ex => OnApplyImageFailed(inner, ex));
            }
        }
        else
        {
            // should hide
            image.HideAsync(token).SafeForget(logger);
        }
    }

    private async Task ApplyImageInternalAsync(Uri? uri, CancellationToken token)
    {
        await HideAsync(token);

        if (uri != null)
        {
            StorageFile storageFile = await imageCache.GetFileFromCacheAsync(uri);
            Compositor compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;

            LoadedImageSurface? imageSurface = null;
            try
            {
                imageSurface = await LoadImageSurfaceAsync(storageFile, token);
            }
            catch (COMException ex) when (ex.Is(COMError.WINCODEC_ERR_COMPONENTNOTFOUND))
            {
                // Image is broken, remove it
                await imageCache.RemoveAsync(uri.Enumerate());
            }

            if (imageSurface != null)
            {
                spriteVisual = CompositeSpriteVisual(compositor, imageSurface);
                OnUpdateVisual(spriteVisual);

                ElementCompositionPreview.SetElementChildVisual(this, spriteVisual);
                await ShowAsync(token);
            }
        }
    }

    private Task ShowAsync(CancellationToken token)
    {
        return AnimationBuilder.Create().Opacity(1d).StartAsync(this, token);
    }

    private Task HideAsync(CancellationToken token)
    {
        return AnimationBuilder.Create().Opacity(0d).StartAsync(this, token);
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (e.NewSize != e.PreviousSize && spriteVisual != null)
        {
            OnUpdateVisual(spriteVisual);
        }
    }
}