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
    private readonly NamedPipeClientStream clientStream = new(".", PrivateNamedPipe.Name, PipeDirection.InOut, PipeOptions.Asynchronous | PipeOptions.WriteThrough);
    private readonly RuntimeOptions runtimeOptions;

    public unsafe bool TryRedirectActivationTo(AppActivationArguments args)
    {
        if (clientStream.TryConnectOnce())
        {
            clientStream.WritePacket(PrivateNamedPipe.Version, PipePacketType.Request, PipePacketCommand.RequestElevationStatus);
            clientStream.ReadPacket(stackalloc byte[sizeof(PipePacketHeader)], out ElevationStatusResponse? response);
            ArgumentNullException.ThrowIfNull(response);

            // Prefer elevated instance
            if (runtimeOptions.IsElevated && !response.IsElevated)
            {
                // Notify previous instance to exit
                clientStream.WritePacket(PrivateNamedPipe.Version, PipePacketType.SessionTermination, PipePacketCommand.Exit);
                clientStream.Flush();
                return false;
            }

            // Redirect to previous instance
            HutaoActivationArguments hutaoArgs = HutaoActivationArguments.FromAppActivationArguments(args, isRedirected: true);
            clientStream.WritePacketWithJsonContent(PrivateNamedPipe.Version, PipePacketType.Request, PipePacketCommand.RedirectActivation, hutaoArgs);
            clientStream.WritePacket(PrivateNamedPipe.Version, PipePacketType.SessionTermination, PipePacketCommand.None);
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