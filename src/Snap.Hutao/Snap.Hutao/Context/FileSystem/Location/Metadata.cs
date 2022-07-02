// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.IO;

namespace Snap.Hutao.Context.FileSystem.Location;

/// <summary>
/// 我的文档位置
/// </summary>
[Injection(InjectAs.Transient)]
public class Metadata : IFileSystemLocation
{
    private string? path;

    /// <inheritdoc/>
    public string GetPath()
    {
        if (string.IsNullOrEmpty(path))
        {
            string myDocument = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            path = Path.GetFullPath(Path.Combine(myDocument, "Hutao", "Metadata"));
        }

        return path;
    }
}