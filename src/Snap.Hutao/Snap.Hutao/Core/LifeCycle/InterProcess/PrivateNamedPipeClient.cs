// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Windows.AppLifecycle;
using Snap.Hutao.Core.LifeCycle.InterProcess.Model;
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
            {
                // Connect
                PipePacketHeader connectPacket = default;
                connectPacket.Version = 1;
                connectPacket.Type = PipePacketType.Request;
                connectPacket.Command = PipePacketCommand.RequestElevationStatus;

                clientStream.Write(new(&connectPacket, sizeof(PipePacketHeader)));
            }

            clientStream.ReadPacket(out ElevationStatusResponse? serverElevationStatus);
            ArgumentNullException.ThrowIfNull(serverElevationStatus);

            if (runtimeOptions.IsElevated && !serverElevationStatus.IsElevated)
            {
                // Kill previous instance to use current elevated instance
                PipePacketHeader killPacket = default;
                killPacket.Version = 1;
                killPacket.Type = PipePacketType.SessionTermination;
                killPacket.Command = PipePacketCommand.Exit;

                clientStream.Write(new(&killPacket, sizeof(PipePacketHeader)));
                clientStream.Flush();
                return false;
            }

            {
                // Redirect to previous instance
                PipePacketHeader redirectActivationPacket = default;
                redirectActivationPacket.Version = 1;
                redirectActivationPacket.Type = PipePacketType.Request;
                redirectActivationPacket.Command = PipePacketCommand.RedirectActivation;
                redirectActivationPacket.ContentType = PipePacketContentType.Json;

                HutaoActivationArguments hutaoArgs = HutaoActivationArguments.FromAppActivationArguments(args, isRedirected: true);
                byte[] jsonBytes = JsonSerializer.SerializeToUtf8Bytes(hutaoArgs);

                clientStream.WritePacket(&redirectActivationPacket, jsonBytes);
            }

            {
                // Terminate session
                PipePacketHeader terminationPacket = default;
                terminationPacket.Version = 1;
                terminationPacket.Type = PipePacketType.SessionTermination;

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