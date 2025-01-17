// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Diagnostics;
using System.IO;
using System.IO.Pipes;

namespace Snap.Hutao.Core.LifeCycle.InterProcess.Yae;

internal sealed partial class YaeNamedPipeServer : IDisposable
{
    private const string PipeName = "YaeAchievementPipe";

    private readonly ILogger<YaeNamedPipeServer> logger;
    private readonly Process gameProcess;

    private readonly CancellationTokenSource serverTokenSource = new();
    private readonly AsyncLock serverLock = new();

    private readonly NamedPipeServerStream serverStream;

    public YaeNamedPipeServer(IServiceProvider serviceProvider, Process gameProcess)
    {
        logger = serviceProvider.GetRequiredService<ILogger<YaeNamedPipeServer>>();
        this.gameProcess = gameProcess;

        serverStream = new NamedPipeServerStream(PipeName);
    }

    public void Dispose()
    {
        serverTokenSource.Cancel();
        using AsyncLock.Releaser discard = serverLock.LockAsync().GetAwaiter().GetResult();
        serverTokenSource.Dispose();
        serverStream.Dispose();
    }

    public async ValueTask<byte[]> GetDataAsync(YaeDataType targetType)
    {
        using (await serverLock.LockAsync().ConfigureAwait(false))
        {
            try
            {
                await serverStream.WaitForConnectionAsync(serverTokenSource.Token).ConfigureAwait(false);
                logger.LogInformation("Client connected.");
                return RunPacketSession(serverStream, targetType, serverTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
            }
        }

        return [];
    }

    private byte[] RunPacketSession(NamedPipeServerStream serverStream, YaeDataType targetType, CancellationToken token)
    {
        byte[] data = [];
        using BinaryReader reader = new(serverStream);
        while (!gameProcess.HasExited && serverStream.IsConnected && !token.IsCancellationRequested)
        {
            try
            {
                YaeDataType type = (YaeDataType)reader.ReadByte();
                int length = reader.ReadInt32();
                if (type != targetType)
                {
                    _ = reader.ReadBytes(length);
                    continue;
                }

                data = reader.ReadBytes(length);
            }
            catch (EndOfStreamException)
            {
            }
        }

        return data;
    }
}