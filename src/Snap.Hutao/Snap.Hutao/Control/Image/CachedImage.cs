// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Snap.Hutao.Core.Caching;
using Snap.Hutao.Extension;
using System.Runtime.InteropServices;

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
        LazyLoadingThreshold = 500;
    }

    /// <inheritdoc/>
    protected override async Task<ImageSource?> ProvideCachedResourceAsync(Uri imageUri, CancellationToken token)
    {
        IImageCache imageCache = Ioc.Default.GetRequiredService<IImageCache>();

        try
        {
            Verify.Operation(imageUri.Host != string.Empty, "无效的Uri");
            string file = await imageCache.GetFileFromCacheAsync(imageUri).ConfigureAwait(true);

            // check token state to determine whether the operation should be canceled.
            token.ThrowIfCancellationRequested();

            // BitmapImage initialize with a uri will increase image quality and loading speed.
            return new BitmapImage(new(file));
        }
        catch (COMException)
        {
            // The image is corrupted, remove it.
            imageCache.Remove(imageUri.Enumerate());
            return null;
        }
        catch (OperationCanceledException)
        {
            // task was explicitly canceled
            return null;
        }
        catch
        {
            throw;
        }
    }
}
