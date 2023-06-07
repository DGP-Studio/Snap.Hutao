// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.IO;

/// <summary>
/// 流复制状态
/// </summary>
internal sealed class StreamCopyStatus
{
    /// <summary>
    /// 构造一个新的流复制状态
    /// </summary>
    /// <param name="bytesCopied">已复制字节</param>
    /// <param name="totalBytes">总字节数</param>
    public StreamCopyStatus(long bytesCopied, long totalBytes)
    {
        BytesCopied = bytesCopied;
        TotalBytes = totalBytes;
    }

    /// <summary>
    /// 已复制字节
    /// </summary>
    public long BytesCopied { get; }

    /// <summary>
    /// 总字节数
    /// </summary>
    public long TotalBytes { get; }
}