// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.IO;
using System.IO.Hashing;

namespace Snap.Hutao.Core.IO.Hashing;

/// <summary>
/// XXH64 摘要
/// </summary>
internal static class XXH64
{
    /// <summary>
    /// 获取流的 XXH64 摘要
    /// </summary>
    /// <param name="stream">流</param>
    /// <param name="token">取消令牌</param>
    /// <returns>摘要</returns>
    public static async ValueTask<string> HashAsync(Stream stream, CancellationToken token = default)
    {
        XxHash64 xxHash64 = new();
        await xxHash64.AppendAsync(stream, token).ConfigureAwait(false);
        byte[] bytes = xxHash64.GetHashAndReset();
        return System.Convert.ToHexString(bytes);
    }

    /// <summary>
    /// 获取文件的 XXH64 摘要
    /// </summary>
    /// <param name="path">路径</param>
    /// <param name="token">取消令牌</param>
    /// <returns>摘要</returns>
    public static async ValueTask<string> HashFileAsync(string path, CancellationToken token = default)
    {
        using (FileStream stream = File.OpenRead(path))
        {
            return await HashAsync(stream, token).ConfigureAwait(false);
        }
    }
}