// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.IO;
using System.Security.Cryptography;

namespace Snap.Hutao.Core.IO;

/// <summary>
/// 摘要
/// </summary>
[HighQuality]
internal static class Digest
{
    /// <summary>
    /// 异步获取文件 Md5 摘要
    /// </summary>
    /// <param name="filePath">文件路径</param>
    /// <param name="token">取消令牌</param>
    /// <returns>文件 Md5 摘要</returns>
    public static async Task<string> GetFileMD5Async(string filePath, CancellationToken token = default)
    {
        using (FileStream stream = File.OpenRead(filePath))
        {
            return await GetStreamMD5Async(stream, token).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// 获取流的 Md5 摘要
    /// </summary>
    /// <param name="stream">流</param>
    /// <param name="token">取消令牌</param>
    /// <returns>流 Md5 摘要</returns>
    public static async Task<string> GetStreamMD5Async(Stream stream, CancellationToken token = default)
    {
        using (MD5 md5 = MD5.Create())
        {
            byte[] bytes = await md5.ComputeHashAsync(stream, token).ConfigureAwait(false);
            return System.Convert.ToHexString(bytes);
        }
    }
}