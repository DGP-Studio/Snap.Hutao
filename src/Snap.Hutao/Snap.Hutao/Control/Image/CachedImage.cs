// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Snap.Hutao.Control.Extension;
using Snap.Hutao.Core.Caching;
using Snap.Hutao.Core.ExceptionService;
using System.Runtime.InteropServices;

namespace Snap.Hutao.Control.Image;

/// <summary>
/// 缓存图像
/// </summary>
[HighQuality]
internal sealed class CachedImage : Implementation.ImageEx
{
    /// <summary>
    /// 构造一个新的缓存图像
    /// </summary>
    public CachedImage()
    {
        DefaultStyleKey = typeof(CachedImage);
        DefaultStyleResourceUri = "ms-appx:///Control/Image/CachedImage.xaml".ToUri();

        IsCacheEnabled = true;
        EnableLazyLoading = false;
    }

    /// <inheritdoc/>
    protected override async Task<ImageSource?> ProvideCachedResourceAsync(Uri imageUri, CancellationToken token)
    {
        IImageCache imageCache = this.ServiceProvider().GetRequiredService<IImageCache>();

        try
        {
            HutaoException.ThrowIf(string.IsNullOrEmpty(imageUri.Host), SH.ControlImageCachedImageInvalidResourceUri);
            string file = await imageCache.GetFileFromCacheAsync(imageUri).ConfigureAwait(true); // BitmapImage need to be created by main thread.
            token.ThrowIfCancellationRequested(); // check token state to determine whether the operation should be canceled.

            // https://learn.microsoft.com/en-us/windows/uwp/debug-test-perf/optimize-animations-and-media#optimize-image-resources
            // BitmapImage initialize with a uri will increase image quality and loading speed.
            return new BitmapImage(file.ToUri());
        }
        catch (COMException)
        {
            // The image is corrupted, remove it.
            imageCache.Remove(imageUri);
            return default;
        }
    }
}
