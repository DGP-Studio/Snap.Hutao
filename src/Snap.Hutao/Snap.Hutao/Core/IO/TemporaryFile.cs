// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.IO;

namespace Snap.Hutao.Core.IO;

/// <summary>
/// 封装一个临时文件
/// </summary>
internal sealed class TemporaryFile : IDisposable
{
    /// <summary>
    /// 构造一个新的临时文件
    /// </summary>
    public TemporaryFile()
    {
        Path = System.IO.Path.GetTempFileName();
    }

    /// <summary>
    /// 路径
    /// </summary>
    public string Path { get; private set; }

    /// <summary>
    /// 创建临时文件并复制内容
    /// </summary>
    /// <param name="file">源文件</param>
    /// <returns>临时文件</returns>
    public static TemporaryFile? CreateFromFileCopy(string file)
    {
        TemporaryFile temporaryFile = new();
        try
        {
            File.Copy(file, temporaryFile.Path, true);
            return temporaryFile;
        }
        catch (System.Exception)
        {
            return null;
        }
    }

    /// <summary>
    /// 删除临时文件
    /// </summary>
    public void Dispose()
    {
        File.Delete(Path);
    }
}
