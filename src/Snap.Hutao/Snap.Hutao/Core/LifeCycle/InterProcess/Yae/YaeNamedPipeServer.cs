// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Buffers;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;

namespace Snap.Hutao.Core.LifeCycle.InterProcess.Yae;

internal sealed class YaeNamedPipeServer : IAsyncDisposable
{
    private const string PipeName = "YaeAchievementPipe";

    private readonly ILogger<YaeNamedPipeServer> logger;
    private readonly NamedPipeServerStream serverStream;
    private readonly Process gameProcess;

    private readonly CancellationTokenSource serverTokenSource = new();
    private readonly AsyncLock serverLock = new();

    public YaeNamedPipeServer(IServiceProvider serviceProvider, Process gameProcess)
    {
        logger = serviceProvider.GetRequiredService<ILogger<YaeNamedPipeServer>>();
        this.gameProcess = gameProcess;

        // Yae is always running elevated, so we don't need to use ACL method.
        serverStream = new(PipeName);
    }

    public async ValueTask DisposeAsync()
    {
        await serverTokenSource.CancelAsync().ConfigureAwait(false);
        using (await serverLock.LockAsync().ConfigureAwait(false))
        {
            // Discard
        }

        serverTokenSource.Dispose();
        await serverStream.DisposeAsync().ConfigureAwait(false);
    }

    public async ValueTask<YaeData> GetDataAsync(YaeDataKind targetKind)
    {
        using (await serverLock.LockAsync().ConfigureAwait(false))
        {
            try
            {
                await serverStream.WaitForConnectionAsync(serverTokenSource.Token).ConfigureAwait(false);
                logger.LogInformation("Client connected.");
                return GetDataByKind(serverStream, targetKind, serverTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
            }
        }

        return default;
    }

    private static unsafe void ReadPacket(PipeStream stream, out YaePacketHeader header, out YaeData data)
    {
        data = default;

        fixed (YaePacketHeader* pHeader = &header)
        {
            stream.ReadExactly(new(pHeader, sizeof(YaePacketHeader)));
        }

        using (IMemoryOwner<byte> owner = MemoryPool<byte>.Shared.Rent(header.ContentLength))
        {
            Span<byte> span = owner.Memory.Span[..header.ContentLength];
            stream.ReadExactly(span);
            data = new(header, owner);
        }
    }

    private YaeData GetDataByKind(NamedPipeServerStream serverStream, YaeDataKind targetKind, CancellationToken token)
    {
        using BinaryReader reader = new(serverStream);
        while (!gameProcess.HasExited && serverStream.IsConnected && !token.IsCancellationRequested)
        {
            try
            {
                ReadPacket(serverStream, out YaePacketHeader header, out YaeData data);
                if (header.Kind == targetKind)
                {
                    return data;
                }
            }
            catch (EndOfStreamException)
            {
                // Client closed.
            }
        }

        return default;
    }
}