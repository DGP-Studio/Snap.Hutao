// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Windows.AppLifecycle;
using System.IO.Pipes;

namespace Snap.Hutao.Core.LifeCycle.InterProcess;

[Injection(InjectAs.Singleton)]
[ConstructorGenerated]
internal sealed partial class PrivateNamedPipeClient : IDisposable
{
    private readonly NamedPipeClientStream clientStream = new(".", "Snap.Hutao.PrivateNamedPipe", PipeDirection.InOut, PipeOptions.Asynchronous | PipeOptions.WriteThrough);
    private readonly RuntimeOptions runtimeOptions;

    public unsafe bool TryRedirectActivationTo(AppActivationArguments args)
    {
        if (clientStream.TryConnectOnce())
        {
            bool shouldElevate = false;
            {
                PipePacketHeader redirectActivationPacket = default;
                redirectActivationPacket.Version = 1;
                redirectActivationPacket.Type = PipePacketType.Request;
                redirectActivationPacket.Command = PipePacketCommand.RedirectActivation;
                redirectActivationPacket.ContentType = PipePacketContentType.Json;

                HutaoActivationArguments hutaoArgs = HutaoActivationArguments.FromAppActivationArguments(args, isRedirected: true, isElevated: runtimeOptions.IsElevated);
                byte[] jsonBytes = JsonSerializer.SerializeToUtf8Bytes(hutaoArgs);

                clientStream.WritePacket(&redirectActivationPacket, jsonBytes);
            }

            {
                Span<byte> headerSpan = stackalloc byte[sizeof(PipePacketHeader)];
                clientStream.ReadExactly(headerSpan);
                fixed (byte* pHeader = headerSpan)
                {
                    PipePacketHeader* header = (PipePacketHeader*)pHeader;
                    ReadOnlySpan<byte> content = clientStream.GetValidatedContent(header);
                    shouldElevate = JsonSerializer.Deserialize<bool>(content);
                }
            }

            {
                PipePacketHeader terminationPacket = default;
                terminationPacket.Version = 1;
                terminationPacket.Type = PipePacketType.Termination;

                clientStream.Write(new(&terminationPacket, sizeof(PipePacketHeader)));
            }

            clientStream.Flush();
            return !shouldElevate;
        }

        return false;
    }

    public void Dispose()
    {
        clientStream.Dispose();
    }
}