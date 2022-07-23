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
    Task<StorageFile> GetFileFromCacheAsync(Uri uri);

    /// <summary>
    /// Removed items based on uri list passed
    /// </summary>
    /// <param name="uriForCachedItems">Enumerable uri list</param>
    /// <returns>awaitable Task</returns>
    Task RemoveAsync(IEnumerable<Uri> uriForCachedItems);

    /// <summary>
    /// Removes cached files that have expired
    /// </summary>
    /// <param name="duration">Optional timespan to compute whether file has expired or not. If no value is supplied, <see cref="CacheDuration"/> is used.</param>
    /// <returns>awaitable task</returns>
    Task RemoveExpiredAsync(TimeSpan? duration = null);
}