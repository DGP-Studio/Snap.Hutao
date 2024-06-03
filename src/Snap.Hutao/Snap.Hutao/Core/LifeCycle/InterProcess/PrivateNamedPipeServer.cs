// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.LifeCycle.InterProcess.Model;
using System.IO.Pipes;
using System.Security.AccessControl;
using System.Security.Principal;

namespace Snap.Hutao.Core.LifeCycle.InterProcess;

[Injection(InjectAs.Singleton)]
internal sealed partial class PrivateNamedPipeServer : IDisposable
{
    private readonly PrivateNamedPipeMessageDispatcher messageDispatcher;
    private readonly RuntimeOptions runtimeOptions;

    private readonly CancellationTokenSource serverTokenSource = new();
    private readonly SemaphoreSlim serverSemaphore = new(1);

    private readonly NamedPipeServerStream serverStream;

    public PrivateNamedPipeServer(IServiceProvider serviceProvider)
    {
        messageDispatcher = serviceProvider.GetRequiredService<PrivateNamedPipeMessageDispatcher>();
        runtimeOptions = serviceProvider.GetRequiredService<RuntimeOptions>();

        PipeSecurity? pipeSecurity = default;

        if (runtimeOptions.IsElevated)
        {
            SecurityIdentifier everyOne = new(WellKnownSidType.WorldSid, null);

            pipeSecurity = new();
            pipeSecurity.AddAccessRule(new PipeAccessRule(everyOne, PipeAccessRights.FullControl, AccessControlType.Allow));
        }

        serverStream = NamedPipeServerStreamAcl.Create(
            "Snap.Hutao.PrivateNamedPipe",
            PipeDirection.InOut,
            NamedPipeServerStream.MaxAllowedServerInstances,
            PipeTransmissionMode.Byte,
            PipeOptions.Asynchronous | PipeOptions.WriteThrough,
            0,
            0,
            pipeSecurity);
    }

    public void Dispose()
    {
        serverTokenSource.Cancel();
        serverSemaphore.Wait();
        serverSemaphore.Dispose();
        serverTokenSource.Dispose();

        serverStream.Dispose();
    }

    public async ValueTask RunAsync()
    {
        using (await serverSemaphore.EnterAsync(serverTokenSource.Token).ConfigureAwait(false))
        {
            while (!serverTokenSource.IsCancellationRequested)
            {
                try
                {
                    await serverStream.WaitForConnectionAsync(serverTokenSource.Token).ConfigureAwait(false);
                    RunPacketSession(serverStream, serverTokenSource.Token);
                }
                catch (OperationCanceledException)
                {
                }
            }
        }
    }

    private unsafe void RunPacketSession(NamedPipeServerStream serverStream, CancellationToken token)
    {
        while (serverStream.IsConnected && !token.IsCancellationRequested)
        {
            PipePacketHeader header = serverStream.ReadPacket(out HutaoActivationArguments? hutaoArgs);
            switch ((header.Type, header.Command))
            {
                case (PipePacketType.Request, PipePacketCommand.RequestElevationStatus):
                    RespondElevationStatus();
                    break;
                case (PipePacketType.Request, PipePacketCommand.RedirectActivation):
                    messageDispatcher.RedirectActivation(hutaoArgs);
                    break;
                case (PipePacketType.SessionTermination, _):
                    serverStream.Disconnect();
                    if (header.Command is PipePacketCommand.Exit)
                    {
                        messageDispatcher.Exit();
                    }

                    return;
            }
        }

        void RespondElevationStatus()
        {
            PipePacketHeader elevatedPacket = default;
            elevatedPacket.Version = 1;
            elevatedPacket.Type = PipePacketType.Response;
            elevatedPacket.Command = PipePacketCommand.ResponseElevationStatus;
            elevatedPacket.ContentType = PipePacketContentType.Json;

            ElevationStatusResponse resp = new()
            {
                IsElevated = runtimeOptions.IsElevated,
            };

            byte[] elevatedBytes = JsonSerializer.SerializeToUtf8Bytes(resp);
            serverStream.WritePacket(&elevatedPacket, elevatedBytes);
            serverStream.Flush();
        }
    }
}