// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.Caching;

internal interface IImageCacheDownloadOperation
{
    ValueTask DownloadFileAsync(Uri uri, string baseFile);
}