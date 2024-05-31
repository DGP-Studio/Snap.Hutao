// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Diagnostics;
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
        Span<byte> headerSpan = stackalloc byte[sizeof(PipePacketHeader)];
        bool shouldElevate = false;
        bool sessionTerminated = false;
        while (serverStream.IsConnected && !sessionTerminated && !token.IsCancellationRequested)
        {
            serverStream.ReadExactly(headerSpan);
            fixed (byte* pHeader = headerSpan)
            {
                PipePacketHeader* header = (PipePacketHeader*)pHeader;

                switch ((header->Type, header->Command, header->ContentType))
                {
                    case (PipePacketType.Request, PipePacketCommand.RedirectActivation, PipePacketContentType.Json):
                        ReadOnlySpan<byte> content = serverStream.GetValidatedContent(header);
                        HutaoActivationArguments? hutaoArgs = JsonSerializer.Deserialize<HutaoActivationArguments>(content);
                        ArgumentNullException.ThrowIfNull(hutaoArgs);

                        PipePacketHeader responsePacket = default;
                        responsePacket.Version = 1;
                        responsePacket.Type = PipePacketType.Response;
                        responsePacket.ContentType = PipePacketContentType.Json;

                        shouldElevate = !runtimeOptions.IsElevated && hutaoArgs.IsElevated;
                        byte[] jsonBytes = JsonSerializer.SerializeToUtf8Bytes(shouldElevate);
                        serverStream.WritePacket(&responsePacket, jsonBytes);
                        serverStream.Flush();

                        if (!shouldElevate)
                        {
                            messageDispatcher.RedirectActivation(hutaoArgs);
                        }

                        break;
                    case (PipePacketType.Termination, _, _):
                        serverStream.Disconnect();
                        sessionTerminated = true;
                        if (shouldElevate)
                        {
                            Process.GetCurrentProcess().Kill();
                        }

                        return;
                }
            }
        }
    }
}