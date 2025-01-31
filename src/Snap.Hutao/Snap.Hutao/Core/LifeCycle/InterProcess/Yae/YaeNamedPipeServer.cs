// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Buffers;
using System.Collections.Immutable;
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

    public async ValueTask<ImmutableArray<YaeData>> GetDataArrayAsync()
    {
        using (await serverLock.LockAsync().ConfigureAwait(false))
        {
            try
            {
                await serverStream.WaitForConnectionAsync(serverTokenSource.Token).ConfigureAwait(false);
                logger.LogInformation("Client connected.");
                return GetDataArray(serverStream);
            }
            catch (OperationCanceledException)
            {
            }
        }

        return default;
    }

    private static unsafe bool TryReadPacket(PipeStream stream, [NotNullWhen(true)] out YaeData? data)
    {
        using (BinaryReader reader = new(stream, Encoding.UTF8, true))
        {
            data = default;
            YaeDataKind kind = reader.Read<YaeDataKind>();

            int contentLength;
            switch (kind)
            {
                case YaeDataKind.Achievement or YaeDataKind.PlayerStore:
                    contentLength = reader.ReadInt32();
                    break;
                case YaeDataKind.VirtualItem:
                    contentLength = sizeof(YaePropertyTypeValue);
                    break;
                case YaeDataKind.End:
                    contentLength = 0; // We can rent zero-length memory.
                    break;
                default:
                    return false;
            }

            IMemoryOwner<byte> owner = MemoryPool<byte>.Shared.Rent(contentLength);
            reader.ReadExactly(owner.Memory.Span[..contentLength]);
            data = new(kind, owner, contentLength);
            return true;
        }
    }

    private ImmutableArray<YaeData> GetDataArray(NamedPipeServerStream serverStream)
    {
        // This method is never cancelable.
        // Yae will prompt error message if the server is closed.
        ImmutableArray<YaeData>.Builder builder = ImmutableArray.CreateBuilder<YaeData>();
        while (!gameProcess.HasExited && serverStream.IsConnected)
        {
            try
            {
                if (TryReadPacket(serverStream, out YaeData? data))
                {
                    builder.Add(data);
                }
            }
            catch (EndOfStreamException)
            {
                // Client closed.
            }
        }

        return builder.ToImmutable();
    }
}