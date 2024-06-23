﻿// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.IO;

namespace Snap.Hutao.Core.IO.Hashing;

/// <summary>
/// 摘要
/// </summary>
#if NET9_0_OR_GREATER
[Obsolete("Use CryptographicOperations.HashData()")]
#endif
internal static class MD5
{
    /// <summary>
    /// 异步获取文件 MD5 摘要
    /// </summary>
    /// <param name="filePath">文件路径</param>
    /// <param name="token">取消令牌</param>
    /// <returns>文件 Md5 摘要</returns>
    public static async ValueTask<string> HashFileAsync(string filePath, CancellationToken token = default)
    {
        using (FileStream stream = File.OpenRead(filePath))
        {
            return await HashAsync(stream, token).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// 获取流的 MD5 摘要
    /// </summary>
    /// <param name="stream">流</param>
    /// <param name="token">取消令牌</param>
    /// <returns>流 Md5 摘要</returns>
    public static async ValueTask<string> HashAsync(Stream stream, CancellationToken token = default)
    {
        byte[] bytes = await System.Security.Cryptography.MD5.HashDataAsync(stream, token).ConfigureAwait(false);
        return Convert.ToHexString(bytes);
    }
}