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

    private readonly CancellationTokenSource disposeCts = new();
    private readonly AsyncLock disposeLock = new();

    private volatile bool disposed;

    public YaeNamedPipeServer(IServiceProvider serviceProvider, Process gameProcess)
    {
        logger = serviceProvider.GetRequiredService<ILogger<YaeNamedPipeServer>>();
        this.gameProcess = gameProcess;

        // Yae is always running elevated, so we don't need to use ACL method.
        Verify.Operation(HutaoRuntime.IsProcessElevated, "Snap Hutao must be elevated to use Yae.");
        serverStream = new(PipeName);
    }

    public async ValueTask DisposeAsync()
    {
        if (disposed)
        {
            return;
        }

        disposed = true;

        await disposeCts.CancelAsync().ConfigureAwait(false);
        using (await disposeLock.LockAsync().ConfigureAwait(false))
        {
            // Discard
        }

        disposeCts.Dispose();
        await serverStream.DisposeAsync().ConfigureAwait(false);
    }

    public async ValueTask<ImmutableArray<YaeData>> GetDataArrayAsync(CancellationToken token = default)
    {
        ObjectDisposedException.ThrowIf(disposed, "YaeNamedPipeServer is disposed.");

        using (await disposeLock.LockAsync().ConfigureAwait(false))
        {
            while (true)
            {
                try
                {
                    using (CancellationTokenSource cts = CancellationTokenSource.CreateLinkedTokenSource(disposeCts.Token, token))
                    {
                        await serverStream.WaitForConnectionAsync(cts.Token).ConfigureAwait(false);
                        logger.LogInformation("Client connected.");
                        return GetDataArray(serverStream);
                    }
                }
                catch (IOException)
                {
                    try
                    {
                        serverStream.Disconnect();
                    }
                    catch
                    {
                        // Ignore
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
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

            IMemoryOwner<byte> owner = MemoryPool<byte>.Shared.RentExactly(contentLength);
            reader.ReadExactly(owner.Memory.Span);
            data = new(kind, owner, contentLength);
            return true;
        }
    }

    private ImmutableArray<YaeData> GetDataArray(NamedPipeServerStream serverStream)
    {
        Debug.Assert(Thread.CurrentThread.IsBackground);

        // This method is never cancellable.
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