// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.IO;

namespace Snap.Hutao.Core;

internal static class RuntimeOptionsExtension
{
    public static string GetDataFolderUpdateCacheFolderFile(this RuntimeOptions options, string fileName)
    {
        string directory = Path.Combine(options.DataFolder, "UpdateCache");
        Directory.CreateDirectory(directory);
        return Path.Combine(directory, fileName);
    }

    public static string GetDataFolderServerCacheFolder(this RuntimeOptions options)
    {
        return Path.Combine(options.DataFolder, "ServerCache");
    }
}