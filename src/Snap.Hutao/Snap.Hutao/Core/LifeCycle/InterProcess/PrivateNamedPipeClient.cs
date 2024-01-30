// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Windows.AppLifecycle;
using System.IO.Hashing;
using System.IO.Pipes;

namespace Snap.Hutao.Core.LifeCycle.InterProcess;

[Injection(InjectAs.Singleton)]
internal sealed class PrivateNamedPipeClient : IDisposable
{
    private readonly NamedPipeClientStream clientStream = new(".", "Snap.Hutao.PrivateNamedPipe", PipeDirection.InOut, PipeOptions.Asynchronous | PipeOptions.WriteThrough);

    public unsafe bool TryRedirectActivationTo(AppActivationArguments args)
    {
        if (clientStream.TryConnectOnce())
        {
            {
                PipePacketHeader redirectActivationPacket = default;
                redirectActivationPacket.Version = 1;
                redirectActivationPacket.Type = PipePacketType.Request;
                redirectActivationPacket.Command = PipePacketCommand.RedirectActivation;
                redirectActivationPacket.ContentType = PipePacketContentType.Json;

                HutaoActivationArguments hutaoArgs = HutaoActivationArguments.FromAppActivationArguments(args, isRedirected: true);
                byte[] jsonBytes = JsonSerializer.SerializeToUtf8Bytes(hutaoArgs);

                redirectActivationPacket.ContentLength = jsonBytes.Length;
                redirectActivationPacket.Checksum = XxHash64.HashToUInt64(jsonBytes);

                clientStream.Write(new(&redirectActivationPacket, sizeof(PipePacketHeader)));
                clientStream.Write(jsonBytes);
            }

            {
                PipePacketHeader terminationPacket = default;
                terminationPacket.Version = 1;
                terminationPacket.Type = PipePacketType.Termination;

                clientStream.Write(new(&terminationPacket, sizeof(PipePacketHeader)));
            }

            clientStream.Flush();
            return true;
        }

        return false;
    }

    public void Dispose()
    {
        clientStream.Dispose();
    }
}