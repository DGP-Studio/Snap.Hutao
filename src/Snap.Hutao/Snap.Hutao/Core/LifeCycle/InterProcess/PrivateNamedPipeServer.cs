// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.ExceptionService;
using System.IO.Hashing;
using System.IO.Pipes;

namespace Snap.Hutao.Core.LifeCycle.InterProcess;

[Injection(InjectAs.Singleton)]
[ConstructorGenerated]
internal sealed partial class PrivateNamedPipeServer : IDisposable
{
    private readonly PrivateNamedPipeMessageDispatcher messageDispatcher;

    private readonly NamedPipeServerStream serverStream = new("Snap.Hutao.PrivateNamedPipe", PipeDirection.InOut, NamedPipeServerStream.MaxAllowedServerInstances, PipeTransmissionMode.Byte, PipeOptions.Asynchronous | PipeOptions.WriteThrough);
    private readonly CancellationTokenSource serverTokenSource = new();
    private readonly SemaphoreSlim serverSemaphore = new(1);

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

    private static unsafe byte[] GetValidatedContent(NamedPipeServerStream serverStream, PipePacketHeader* header)
    {
        byte[] content = new byte[header->ContentLength];
        serverStream.ReadAtLeast(content, header->ContentLength, false);
        ThrowHelper.InvalidDataIf(XxHash64.HashToUInt64(content) != header->Checksum, "PipePacket Content Hash incorrect");
        return content;
    }

    private unsafe void RunPacketSession(NamedPipeServerStream serverStream, CancellationToken token)
    {
        Span<byte> headerSpan = stackalloc byte[sizeof(PipePacketHeader)];
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
                        ReadOnlySpan<byte> content = GetValidatedContent(serverStream, header);
                        messageDispatcher.RedirectActivation(JsonSerializer.Deserialize<HutaoActivationArguments>(content));
                        break;
                    case (PipePacketType.Termination, _, _):
                        serverStream.Disconnect();
                        sessionTerminated = true;
                        return;
                }
            }
        }
    }
}