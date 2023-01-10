// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.IO.Bits;

/// <summary>
/// 进度更新状态
/// </summary>
public class ProgressUpdateStatus
{
    /// <summary>
    /// 构造一个新的进度更新状态
    /// </summary>
    /// <param name="bytesRead">接收字节数</param>
    /// <param name="totalBytes">总字节数</param>
    public ProgressUpdateStatus(long bytesRead, long totalBytes)
    {
        BytesRead = bytesRead;
        TotalBytes = totalBytes;
    }

    /// <summary>
    /// 接收字节数
    /// </summary>
    public long BytesRead { get; private set; }

    /// <summary>
    /// 总字节数
    /// </summary>
    public long TotalBytes { get; private set; }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{BytesRead}/{TotalBytes}";
    }
}