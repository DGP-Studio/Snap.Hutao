// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Media.Imaging;
using System.Collections.Generic;
using Windows.Storage;

namespace Snap.Hutao.Core.Caching;

/// <summary>
/// 为图像缓存提供抽象
/// </summary>
/// <typeparam name="T">缓存类型</typeparam>
internal interface IImageCache
{
    /// <summary>
    /// Gets the StorageFile containing cached item for given Uri
    /// </summary>
    /// <param name="uri">Uri of the item.</param>
    /// <returns>a StorageFile</returns>
    Task<StorageFile?> GetFileFromCacheAsync(Uri uri);

    /// <summary>
    /// Retrieves item represented by Uri from the cache. If the item is not found in the cache, it will try to downloaded and saved before returning it to the caller.
    /// </summary>
    /// <param name="uri">Uri of the item.</param>
    /// <param name="throwOnError">Indicates whether or not exception should be thrown if item cannot be found / downloaded.</param>
    /// <returns>an instance of Generic type</returns>
    Task<BitmapImage?> GetFromCacheAsync(Uri uri, bool throwOnError = false);

    /// <summary>
    /// Removed items based on uri list passed
    /// </summary>
    /// <param name="uriForCachedItems">Enumerable uri list</param>
    /// <returns>awaitable Task</returns>
    Task RemoveAsync(IEnumerable<Uri> uriForCachedItems);
}