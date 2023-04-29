// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.IO;

namespace Snap.Hutao.Core.IO;

/// <summary>
/// 流复制器
/// </summary>
internal sealed class StreamCopyWorker
{
    private readonly Stream source;
    private readonly Stream destination;
    private readonly long totalBytes;
    private readonly int bufferSize;

    /// <summary>
    /// 创建一个新的流复制器
    /// </summary>
    /// <param name="source">源</param>
    /// <param name="destination">目标</param>
    /// <param name="totalBytes">总字节</param>
    /// <param name="bufferSize">字节尺寸</param>
    public StreamCopyWorker(Stream source, Stream destination, long totalBytes, int bufferSize = 81920)
    {
        Verify.Operation(source.CanRead, "Source Stream can't read");
        Verify.Operation(destination.CanWrite, "Destination Stream can't write");

        this.source = source;
        this.destination = destination;
        this.totalBytes = totalBytes;
        this.bufferSize = bufferSize;
    }

    /// <summary>
    /// 异步复制
    /// </summary>
    /// <param name="progress">进度</param>
    /// <returns>任务</returns>
    public async Task CopyAsync(IProgress<StreamCopyState> progress)
    {
        long totalBytesRead = 0;
        int bytesRead;
        Memory<byte> buffer = new byte[bufferSize];

        do
        {
            bytesRead = await source.ReadAsync(buffer).ConfigureAwait(false);
            await destination.WriteAsync(buffer[..bytesRead]).ConfigureAwait(false);

            totalBytesRead += bytesRead;
            progress.Report(new(totalBytesRead, totalBytes));
        }
        while (bytesRead > 0);
    }
}

/// <summary>
/// 针对特定类型的流复制器
/// </summary>
/// <typeparam name="TStatus">进度类型</typeparam>
[SuppressMessage("", "SA1402")]
internal sealed class StreamCopyWorker<TStatus>
{
    private readonly Stream source;
    private readonly Stream destination;
    private readonly int bufferSize;
    private readonly Func<long, TStatus> statusFactory;

    /// <summary>
    /// 创建一个新的流复制器
    /// </summary>
    /// <param name="source">源</param>
    /// <param name="destination">目标</param>
    /// <param name="statusFactory">状态工厂</param>
    /// <param name="totalBytes">总字节</param>
    /// <param name="bufferSize">字节尺寸</param>
    public StreamCopyWorker(Stream source, Stream destination, Func<long, TStatus> statusFactory, int bufferSize = 81920)
    {
        Verify.Operation(source.CanRead, "Source Stream can't read");
        Verify.Operation(destination.CanWrite, "Destination Stream can't write");

        this.source = source;
        this.destination = destination;
        this.statusFactory = statusFactory;
        this.bufferSize = bufferSize;
    }

    /// <summary>
    /// 异步复制
    /// </summary>
    /// <param name="progress">进度</param>
    /// <returns>任务</returns>
    public async Task CopyAsync(IProgress<TStatus> progress)
    {
        long totalBytesRead = 0;
        int bytesRead;
        Memory<byte> buffer = new byte[bufferSize];

        do
        {
            bytesRead = await source.ReadAsync(buffer).ConfigureAwait(false);
            await destination.WriteAsync(buffer[..bytesRead]).ConfigureAwait(false);

            totalBytesRead += bytesRead;
            progress.Report(statusFactory(totalBytesRead));
        }
        while (bytesRead > 0);
    }
}