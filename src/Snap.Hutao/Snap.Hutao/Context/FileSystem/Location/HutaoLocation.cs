// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.IO;
using Windows.ApplicationModel;

namespace Snap.Hutao.Context.FileSystem.Location;

/// <summary>
/// 我的文档位置
/// </summary>
[Injection(InjectAs.Transient)]
internal class HutaoLocation : IFileSystemLocation
{
    private string? path;

    /// <inheritdoc/>
    public string GetPath()
    {
        if (string.IsNullOrEmpty(path))
        {
            string myDocument = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            // 将测试版与正式版的文件目录分离
            string folderName = Package.Current.PublisherDisplayName == "DGP Studio CI" ? "HutaoAlpha" : "Hutao";
            path = Path.GetFullPath(Path.Combine(myDocument, folderName));
        }

        return path;
    }
}