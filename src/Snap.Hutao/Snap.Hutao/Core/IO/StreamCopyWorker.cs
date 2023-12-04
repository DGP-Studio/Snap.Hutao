// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Diagnostics;
using System.IO;

namespace Snap.Hutao.Core.IO;

internal sealed class StreamCopyWorker : StreamCopyWorker<StreamCopyStatus>
{
    public StreamCopyWorker(Stream source, Stream destination, long totalBytes, int bufferSize = 81920)
        : base(source, destination, bytes => new StreamCopyStatus(bytes, totalBytes), bufferSize)
    {
    }
}

[SuppressMessage("", "SA1402")]
internal class StreamCopyWorker<TStatus>
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
    public async ValueTask CopyAsync(IProgress<TStatus> progress)
    {
        ValueStopwatch stopwatch = ValueStopwatch.StartNew();

        long totalBytesRead = 0;
        int bytesRead;
        Memory<byte> buffer = new byte[bufferSize];

        do
        {
            bytesRead = await source.ReadAsync(buffer).ConfigureAwait(false);
            if (bytesRead == 0)
            {
                progress.Report(statusFactory(totalBytesRead));
                break;
            }

            await destination.WriteAsync(buffer[..bytesRead]).ConfigureAwait(false);

            totalBytesRead += bytesRead;
            if (stopwatch.GetElapsedTime().TotalMilliseconds > 500)
            {
                progress.Report(statusFactory(totalBytesRead));
                stopwatch = ValueStopwatch.StartNew();
            }
        }
        while (bytesRead > 0);
    }
}