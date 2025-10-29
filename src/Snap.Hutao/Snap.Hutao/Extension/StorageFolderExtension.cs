// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.IO;
using Windows.Storage;

namespace Snap.Hutao.Extension;

internal static class StorageFolderExtension
{
    public static async ValueTask<StorageFolder> CopyAsync(
        this StorageFolder sourceFolder,
        string targetFolderFullPath,
        NameCollisionOption nameCollisionOption = NameCollisionOption.ReplaceExisting,
        CreationCollisionOption creationCollisionOption = CreationCollisionOption.ReplaceExisting)
    {
        Directory.CreateDirectory(targetFolderFullPath);
        StorageFolder targetFolder = await StorageFolder.GetFolderFromPathAsync(targetFolderFullPath);
        await sourceFolder.CopyAsync(targetFolder, nameCollisionOption, creationCollisionOption).ConfigureAwait(false);
        return targetFolder;
    }

    public static async ValueTask CopyAsync(
        this StorageFolder sourceFolder,
        StorageFolder targetFolder,
        NameCollisionOption nameCollisionOption = NameCollisionOption.ReplaceExisting,
        CreationCollisionOption creationCollisionOption = CreationCollisionOption.OpenIfExists)
    {
        foreach (StorageFolder folder in await sourceFolder.GetFoldersAsync())
        {
            StorageFolder subFolder = await targetFolder.CreateFolderAsync(folder.Name, creationCollisionOption);
            await folder.CopyAsync(subFolder).ConfigureAwait(false);
        }

        foreach (StorageFile file in await sourceFolder.GetFilesAsync())
        {
            await file.CopyAsync(targetFolder, file.Name, nameCollisionOption);
        }
    }
}