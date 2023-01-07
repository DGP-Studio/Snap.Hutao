// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Windows.Storage;

namespace Snap.Hutao.Core.Caching;

/// <summary>
/// 为图像缓存提供抽象
/// </summary>
/// <typeparam name="T">缓存类型</typeparam>
internal interface IImageCache
{
    /// <summary>
    /// Gets the file path containing cached item for given Uri
    /// </summary>
    /// <param name="uri">Uri of the item.</param>
    /// <returns>a string path</returns>
    Task<string> GetFileFromCacheAsync(Uri uri);

    /// <summary>
    /// Removed items based on uri list passed
    /// </summary>
    /// <param name="uriForCachedItems">Enumerable uri list</param>
    void Remove(IEnumerable<Uri> uriForCachedItems);

    /// <summary>
    /// Removes invalid cached files
    /// </summary>
    void RemoveInvalid();
}