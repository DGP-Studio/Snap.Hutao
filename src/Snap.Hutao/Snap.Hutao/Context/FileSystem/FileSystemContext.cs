// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Context.FileSystem.Location;
using System.IO;

namespace Snap.Hutao.Context.FileSystem;

/// <summary>
/// 文件系统上下文
/// </summary>
/// <typeparam name="TLocation">路径位置类型</typeparam>
internal abstract class FileSystemContext
{
    private readonly IFileSystemLocation location;

    /// <summary>
    /// 初始化文件系统上下文
    /// </summary>
    /// <param name="location">指定的文件系统位置</param>
    public FileSystemContext(IFileSystemLocation location)
    {
        this.location = location;
        EnsureDirectory();
    }

    /// <summary>
    /// 创建文件，若已存在文件，则不会创建
    /// </summary>
    /// <param name="file">文件</param>
    public void CreateFileOrIgnore(string file)
    {
        file = Locate(file);
        if (!File.Exists(file))
        {
            File.Create(file).Dispose();
        }
    }

    /// <summary>
    /// 创建文件夹，若已存在文件，则不会创建
    /// </summary>
    /// <param name="folder">文件夹</param>
    public void CreateFolderOrIgnore(string folder)
    {
        folder = Locate(folder);
        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
        }
    }

    /// <summary>
    /// 尝试删除文件夹
    /// </summary>
    /// <param name="folder">文件夹</param>
    public void DeleteFolderOrIgnore(string folder)
    {
        folder = Locate(folder);
        if (Directory.Exists(folder))
        {
            Directory.Delete(folder, true);
        }
    }

    /// <summary>
    /// 检查文件是否存在
    /// </summary>
    /// <param name="file">文件名称</param>
    /// <returns>是否存在</returns>
    public bool FileExists(string file)
    {
        return File.Exists(Locate(file));
    }

    /// <summary>
    /// 检查文件是否存在
    /// </summary>
    /// <param name="folder">文件夹名称</param>
    /// <param name="file">文件名称</param>
    /// <returns>是否存在</returns>
    public bool FileExists(string folder, string file)
    {
        return File.Exists(Locate(folder, file));
    }

    /// <summary>
    /// 检查文件是否存在
    /// </summary>
    /// <param name="folder">文件夹名称</param>
    /// <returns>是否存在</returns>
    public bool FolderExists(string folder)
    {
        return Directory.Exists(Locate(folder));
    }

    /// <summary>
    /// 定位根目录中的文件或文件夹
    /// </summary>
    /// <param name="fileOrFolder">文件或文件夹</param>
    /// <returns>绝对路径</returns>
    public string Locate(string fileOrFolder)
    {
        return Path.GetFullPath(fileOrFolder, location.GetPath());
    }

    /// <summary>
    /// 定位根目录下子文件夹中的文件
    /// </summary>
    /// <param name="folder">文件夹</param>
    /// <param name="file">文件</param>
    /// <returns>绝对路径</returns>
    public string Locate(string folder, string file)
    {
        return Path.GetFullPath(Path.Combine(folder, file), location.GetPath());
    }

    /// <summary>
    /// 将文件移动到指定的子目录
    /// </summary>
    /// <param name="file">文件</param>
    /// <param name="folder">文件夹</param>
    /// <param name="overwrite">是否覆盖</param>
    /// <returns>是否成功 当文件不存在时会失败</returns>
    public bool MoveToFolderOrIgnore(string file, string folder, bool overwrite = true)
    {
        string target = Locate(folder, file);
        file = Locate(file);

        if (File.Exists(file))
        {
            File.Move(file, target, overwrite);
            return true;
        }

        return false;
    }

    /// <summary>
    /// 等效于 <see cref="File.OpenRead(string)"/> ，但路径经过解析
    /// </summary>
    /// <param name="file">文件名</param>
    /// <returns>文件流</returns>
    public FileStream OpenRead(string file)
    {
        return File.OpenRead(Locate(file));
    }

    /// <summary>
    /// 等效于 <see cref="File.Create(string)"/> ，但路径经过解析
    /// </summary>
    /// <param name="file">文件名</param>
    /// <returns>文件流</returns>
    public FileStream Create(string file)
    {
        return File.Create(Locate(file));
    }

    /// <summary>
    /// 检查根目录
    /// </summary>
    /// <returns>是否创建了路径</returns>
    private bool EnsureDirectory()
    {
        string folder = location.GetPath();
        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
            return true;
        }

        return false;
    }
}