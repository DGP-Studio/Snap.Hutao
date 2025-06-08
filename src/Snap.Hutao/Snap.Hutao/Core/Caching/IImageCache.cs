// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Snap.Hutao.Core.IO;

namespace Snap.Hutao.Core.Caching;

internal interface IImageCache : IImageCacheFilePathOperation
{
    ValueTask<ValueFile> GetFileFromCacheAsync(Uri uri);

    ValueTask<ValueFile> GetFileFromCacheAsync(Uri uri, ElementTheme theme);

    void Remove(Uri uriForCachedItem);
}