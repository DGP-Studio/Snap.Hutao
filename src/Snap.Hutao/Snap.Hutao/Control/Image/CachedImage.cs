// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Snap.Hutao.Core.Caching;
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
        IsCacheEnabled = true;
        EnableLazyLoading = true;
    }

    /// <inheritdoc/>
    protected override async Task<ImageSource?> ProvideCachedResourceAsync(Uri imageUri, CancellationToken token)
    {
        // We can only use Ioc to retrieve IImageCache, no IServiceProvider is available.
        IImageCache imageCache = Ioc.Default.GetRequiredService<IImageCache>();

        try
        {
            Verify.Operation(!string.IsNullOrEmpty(imageUri.Host), SH.ControlImageCachedImageInvalidResourceUri);
            string file = await imageCache.GetFileFromCacheAsync(imageUri).ConfigureAwait(true); // BitmapImage need to be created by main thread.
            token.ThrowIfCancellationRequested(); // check token state to determine whether the operation should be canceled.
            return new BitmapImage(file.ToUri()); // BitmapImage initialize with a uri will increase image quality and loading speed.
        }
        catch (COMException)
        {
            // The image is corrupted, remove it.
            imageCache.Remove(imageUri);
            return null;
        }
        catch (OperationCanceledException)
        {
            // task was explicitly canceled
            return null;
        }
    }
}
