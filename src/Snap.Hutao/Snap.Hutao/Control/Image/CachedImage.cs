// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI.UI;
using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Snap.Hutao.Extension;

namespace Snap.Hutao.Control.Image;

/// <summary>
/// 缓存图像
/// </summary>
public class CachedImage : ImageEx
{
    static CachedImage()
    {
        ImageCache.Instance.CacheDuration = Timeout.InfiniteTimeSpan;
        ImageCache.Instance.RetryCount = 3;
    }

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
        BitmapImage image;
        try
        {
            image = await ImageCache.Instance.GetFromCacheAsync(imageUri, true, token);
        }
        catch
        {
            // maybe the image is corrupted remove it and re-download
            await ImageCache.Instance.RemoveAsync(imageUri.Enumerate());
            image = await ImageCache.Instance.GetFromCacheAsync(imageUri, false, token);
        }

        // check token state to determine whether the operation should be canceled.
        if (token.IsCancellationRequested)
        {
            throw new TaskCanceledException("Image source has changed.");
        }
        else
        {
            return Must.NotNull(image);
        }
    }
}
