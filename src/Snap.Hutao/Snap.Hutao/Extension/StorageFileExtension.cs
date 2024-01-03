// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.IO;
using Windows.Storage;

namespace Snap.Hutao.Extension;

internal static class StorageFileExtension
{
    public static async ValueTask OverwriteCopyAsync(this StorageFile file, string targetFile)
    {
        using (Stream outputStream = (await file.OpenReadAsync()).AsStreamForRead())
        {
            using (FileStream inputStream = File.Create(targetFile))
            {
                await outputStream.CopyToAsync(inputStream).ConfigureAwait(false);
            }
        }
    }
}