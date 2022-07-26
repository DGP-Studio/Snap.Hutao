// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI.UI.Animations;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Hosting;
using Microsoft.UI.Xaml.Media;
using Snap.Hutao.Core;
using Snap.Hutao.Core.Caching;
using Snap.Hutao.Core.Threading;
using Snap.Hutao.Extension;
using Snap.Hutao.Service.Abstraction;
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

    private SpriteVisual? spriteVisual;

    /// <summary>
    /// 构造一个新的单色图像
    /// </summary>
    public CompositionImage()
    {
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

    private static Task<StorageFile> GetCachedFileAsync(Uri uri)
    {
        return Ioc.Default.GetRequiredService<IImageCache>().GetFileFromCacheAsync(uri);
    }

    private static void OnApplyImageFailed(Exception exception)
    {
        Ioc.Default
            .GetRequiredService<IInfoBarService>()
            .Error(exception, "应用合成图像时发生异常");
    }

    private static void OnSourceChanged(DependencyObject sender, DependencyPropertyChangedEventArgs arg)
    {
        if (arg.NewValue is Uri uri && uri != (arg.OldValue as Uri) && !string.IsNullOrEmpty(uri.Host))
        {
            CompositionImage image = (CompositionImage)sender;
            ILogger<CompositionImage> logger = Ioc.Default.GetRequiredService<ILogger<CompositionImage>>();
            image.ApplyImageInternalAsync(uri, LoadingTokenSource.Register(image)).SafeForget(logger, OnApplyImageFailed);
        }
    }

    private async Task ApplyImageInternalAsync(Uri? uri, CancellationToken token)
    {
        await AnimationBuilder.Create().Opacity(0d).StartAsync(this, token);

        if (uri is null)
        {
            return;
        }

        StorageFile storageFile = await GetCachedFileAsync(uri);

        Compositor compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;

        LoadedImageSurface imageSurface = await LoadImageSurfaceAsync(storageFile, token);

        spriteVisual = CompositeSpriteVisual(compositor, imageSurface);
        OnUpdateVisual(spriteVisual);

        ElementCompositionPreview.SetElementChildVisual(this, spriteVisual);

        await AnimationBuilder.Create().Opacity(1d).StartAsync(this, token);
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (e.NewSize != e.PreviousSize && spriteVisual is not null)
        {
            OnUpdateVisual(spriteVisual);
        }
    }
}