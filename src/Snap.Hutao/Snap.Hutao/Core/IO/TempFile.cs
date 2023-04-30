// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.ExceptionService;
using System.IO;

namespace Snap.Hutao.Core.IO;

/// <summary>
/// 封装一个临时文件
/// </summary>
[HighQuality]
internal sealed class TempFile : IDisposable
{
    /// <summary>
    /// 构造一个新的临时文件
    /// </summary>
    /// <param name="delete">是否在创建时删除文件</param>
    public TempFile(bool delete = false)
    {
        try
        {
            Path = System.IO.Path.GetTempFileName();
        }
        catch (UnauthorizedAccessException ex)
        {
            ThrowHelper.RuntimeEnvironment(SH.CoreIOTempFileCreateFail, ex);
        }

        if (delete)
        {
            File.Delete(Path);
        }
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
    public static TempFile? CopyFrom(string file)
    {
        TempFile temporaryFile = new();
        try
        {
            File.Copy(file, temporaryFile.Path, true);
            return temporaryFile;
        }
        catch (Exception)
        {
            return null;
        }
    }

    /// <summary>
    /// 作为写入模式打开
    /// </summary>
    /// <returns>文件流</returns>
    public FileStream OpenWrite()
    {
        return File.OpenWrite(Path);
    }

    /// <summary>
    /// 删除临时文件
    /// </summary>
    public void Dispose()
    {
        try
        {
            File.Delete(Path);
        }
        catch (IOException)
        {
        }
    }
}