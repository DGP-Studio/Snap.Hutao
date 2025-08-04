// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Game.Launching.Handler;
using Snap.Hutao.Service.Yae.Achievement;
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
    private readonly TargetNativeConfiguration config;

    private readonly CancellationTokenSource disposeCts = new();
    private readonly AsyncLock disposeLock = new();

    private volatile bool disposed;

    public YaeNamedPipeServer(IServiceProvider serviceProvider, Process gameProcess, TargetNativeConfiguration config)
    {
        Verify.Operation(HutaoRuntime.IsProcessElevated, "Snap Hutao must be elevated to use Yae.");

        logger = serviceProvider.GetRequiredService<ILogger<YaeNamedPipeServer>>();
        this.gameProcess = gameProcess;
        this.config = config;

        // Yae is always running elevated, so we don't need to use ACL method.
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
            while (LaunchExecutionEnsureGameNotRunningHandler.IsGameRunning())
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

    private static unsafe YaeData? HandleCommand(PipeStream stream, TargetNativeConfiguration config)
    {
        using BinaryReader reader = new(stream, Encoding.UTF8, true);
        using BinaryWriter writer = new(stream, Encoding.UTF8, true);

        YaeCommandKind kind = reader.Read<YaeCommandKind>();

        switch (kind)
        {
            case YaeCommandKind.RequestCmdId:
                {
                    writer.Write(config.AchievementCmdId);
                    writer.Write(config.StoreCmdId);
                    return default;
                }

            case YaeCommandKind.RequestRva:
                {
                    writer.Write(config.DoCmd);
                    writer.Write(config.ToUInt16);
                    writer.Write(config.UpdateNormalProperty);
                    return default;
                }

            case YaeCommandKind.ResponseAchievement or YaeCommandKind.ResponsePlayerStore:
                {
                    int contentLength = reader.ReadInt32();
                    IMemoryOwner<byte> owner = MemoryPool<byte>.Shared.RentExactly(contentLength);
                    reader.ReadExactly(owner.Memory.Span);
                    return new(kind, owner, contentLength);
                }

            case YaeCommandKind.ResponseVirtualItem:
                {
                    int contentLength = sizeof(YaePropertyTypeValue);
                    IMemoryOwner<byte> owner = MemoryPool<byte>.Shared.RentExactly(contentLength);
                    reader.ReadExactly(owner.Memory.Span);
                    return new(kind, owner, contentLength);
                }

            default:
                return default;
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
                if (HandleCommand(serverStream, config) is { } data)
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