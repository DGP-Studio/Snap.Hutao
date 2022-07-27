// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Snap.Hutao.Core.Caching;
using Snap.Hutao.Extension;
using Windows.Storage;

namespace Snap.Hutao.Control.Image;

/// <summary>
/// 缓存图像
/// </summary>
public class CachedImage : ImageEx
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
    protected override async Task<ImageSource> ProvideCachedResourceAsync(Uri imageUri, CancellationToken token)
    {
        IImageCache imageCache = Ioc.Default.GetRequiredService<IImageCache>();

        try
        {
            Verify.Operation(imageUri.Host != string.Empty, "可能是空绑定产生的 [ms-appx:///]");
            StorageFile file = await imageCache.GetFileFromCacheAsync(imageUri);

            // check token state to determine whether the operation should be canceled.
            Must.TryThrowOnCanceled(token, "Image source has changed.");
            return new BitmapImage(new(file.Path));
        }
        catch (TaskCanceledException)
        {
            // task was explicitly canceled
            throw;
        }
        catch
        {
            // maybe the image is corrupted, remove it.
            await imageCache.RemoveAsync(imageUri.Enumerate()).ConfigureAwait(false);
            throw;
        }
    }
}
