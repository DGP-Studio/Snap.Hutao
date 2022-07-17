// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.IO;

namespace Snap.Hutao.Context.FileSystem.Location;

/// <summary>
/// 缓存位置
/// </summary>
[Injection(InjectAs.Transient)]
internal class Cache : IFileSystemLocation
{
    private string? path;

    /// <inheritdoc/>
    public string GetPath()
    {
        if (string.IsNullOrEmpty(path))
        {
            path = Windows.Storage.ApplicationData.Current.TemporaryFolder.Path;
        }

        return path;
    }
}