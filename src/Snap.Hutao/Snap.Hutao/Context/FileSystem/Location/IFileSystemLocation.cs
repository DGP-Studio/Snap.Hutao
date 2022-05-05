// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Context.FileSystem.Location;

/// <summary>
/// 文件系统位置
/// </summary>
public interface IFileSystemLocation
{
    /// <summary>
    /// 获取路径
    /// </summary>
    /// <returns>路径</returns>
    string GetPath();
}