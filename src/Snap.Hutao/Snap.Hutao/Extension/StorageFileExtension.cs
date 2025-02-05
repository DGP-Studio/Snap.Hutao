// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.IO;
using Windows.Storage;

namespace Snap.Hutao.Extension;

internal static class StorageFileExtension
{
    public static async ValueTask<StorageFile> CopyAsync(this StorageFile sourceFile, string targetFileFullPath, NameCollisionOption option = NameCollisionOption.ReplaceExisting)
    {
        StorageFolder targetFolder = await StorageFolder.GetFolderFromPathAsync(Path.GetDirectoryName(targetFileFullPath));
        return await sourceFile.CopyAsync(targetFolder, Path.GetFileName(targetFileFullPath), option);
    }
}