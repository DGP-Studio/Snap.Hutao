// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Buffers;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Text;

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

    private static unsafe void ReadPacket(PipeStream stream, out YaeData data)
    {
        data = default;

        YaeDataKind kind = default;
        stream.ReadExactly(new(&kind, sizeof(YaeDataKind)));

        if (kind is YaeDataKind.Achievement or YaeDataKind.PlayerStore)
        {
            int contentLength = default;
            stream.ReadExactly(new(&contentLength, sizeof(int)));

            IMemoryOwner<byte> owner = MemoryPool<byte>.Shared.Rent(contentLength);
            Span<byte> span = owner.Memory.Span[..contentLength];
            stream.ReadExactly(span);
            data = new(kind, owner, contentLength);
        }
        else if (kind is YaeDataKind.VirtualItem)
        {
            // TODO
        }
    }

    private YaeData GetDataByKind(NamedPipeServerStream serverStream, YaeDataKind targetKind, CancellationToken token)
    {
        YaeData result = default;
        while (!gameProcess.HasExited && serverStream.IsConnected && !token.IsCancellationRequested)
        {
            try
            {
                ReadPacket(serverStream, out YaeData data);
                if (data.Kind == targetKind)
                {
                    result = data;
                }
            }
            catch (EndOfStreamException)
            {
                // Client closed.
            }
        }

        return result;
    }
}