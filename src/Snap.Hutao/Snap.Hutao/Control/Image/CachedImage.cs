// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Snap.Hutao.Core.Caching;
using Snap.Hutao.Extension;

namespace Snap.Hutao.Control.Image;

/// <summary>
/// 缓存图像
/// </summary>
public class CachedImage : ImageEx
{
    private readonly IImageCache imageCache;

    /// <summary>
    /// 构造一个新的缓存图像
    /// </summary>
    public CachedImage()
    {
        imageCache = Ioc.Default.GetRequiredService<IImageCache>();

        IsCacheEnabled = true;
        EnableLazyLoading = true;
    }

    /// <inheritdoc/>
    protected override async Task<ImageSource> ProvideCachedResourceAsync(Uri imageUri, CancellationToken token)
    {
        BitmapImage? image;
        try
        {
            image = await imageCache.GetFromCacheAsync(imageUri, true);
        }
        catch (TaskCanceledException)
        {
            // task was explicitly canceled
            throw;
        }
        catch
        {
            // maybe the image is corrupted, remove it.
            await imageCache.RemoveAsync(imageUri.Enumerate());
            throw;
        }

        // check token state to determine whether the operation should be canceled.
        if (token.IsCancellationRequested)
        {
            throw new TaskCanceledException("Image source has changed.");
        }
        else
        {
            return Must.NotNull(image!);
        }
    }
}
