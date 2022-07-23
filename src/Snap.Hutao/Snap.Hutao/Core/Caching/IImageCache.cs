// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Media.Imaging;
using System.Collections.Generic;

namespace Snap.Hutao.Core.Caching;

/// <summary>
/// 为图像缓存提供抽象
/// </summary>
/// <typeparam name="T">缓存类型</typeparam>
internal interface IImageCache
{
    /// <summary>
    /// Retrieves item represented by Uri from the cache. If the item is not found in the cache, it will try to downloaded and saved before returning it to the caller.
    /// </summary>
    /// <param name="uri">Uri of the item.</param>
    /// <param name="throwOnError">Indicates whether or not exception should be thrown if item cannot be found / downloaded.</param>
    /// <param name="cancellationToken">instance of <see cref="CancellationToken"/></param>
    /// <param name="initializerKeyValues">key value pairs used when initializing instance of generic type</param>
    /// <returns>an instance of Generic type</returns>
    [SuppressMessage("", "CA1068")]
    Task<BitmapImage> GetFromCacheAsync(Uri uri, bool throwOnError = false, CancellationToken cancellationToken = default(CancellationToken), List<KeyValuePair<string, object>> initializerKeyValues = null!);

    /// <summary>
    /// Removed items based on uri list passed
    /// </summary>
    /// <param name="uriForCachedItems">Enumerable uri list</param>
    /// <returns>awaitable Task</returns>
    Task RemoveAsync(IEnumerable<Uri> uriForCachedItems);
}