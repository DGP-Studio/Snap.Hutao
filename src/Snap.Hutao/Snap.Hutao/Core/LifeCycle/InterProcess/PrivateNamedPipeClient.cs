// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Windows.AppLifecycle;
using Snap.Hutao.Core.LifeCycle.InterProcess.Model;
using System.Diagnostics;
using System.IO.Pipes;

namespace Snap.Hutao.Core.LifeCycle.InterProcess;

[Injection(InjectAs.Singleton)]
internal sealed partial class PrivateNamedPipeClient : IDisposable
{
    private readonly NamedPipeClientStream clientStream = new(".", PrivateNamedPipe.Name, PipeDirection.InOut, PipeOptions.Asynchronous | PipeOptions.WriteThrough);

    public bool TryRedirectActivationTo(AppActivationArguments args)
    {
        if (!clientStream.TryConnectOnce())
        {
            return false;
        }

        clientStream.WritePacket(PrivateNamedPipe.Version, PipePacketType.Request, PipePacketCommand.RequestElevationStatus);
        clientStream.ReadPacket(out PipePacketHeader _, out ElevationStatusResponse? response);
        ArgumentNullException.ThrowIfNull(response);

        // Prefer elevated instance
        if (HutaoRuntime.IsProcessElevated && !response.IsElevated)
        {
            // Notify previous instance to exit
            clientStream.WritePacket(PrivateNamedPipe.Version, PipePacketType.SessionTermination, PipePacketCommand.Exit);
            clientStream.Flush();
            WaitPreviousProcessExit(response);

            // Retain the elevated instance
            return false;
        }

        // Redirect to previous instance
        HutaoActivationArguments hutaoArgs = HutaoActivationArguments.FromAppActivationArguments(args, isRedirected: true);
        clientStream.WritePacketWithJsonContent(PrivateNamedPipe.Version, PipePacketType.Request, PipePacketCommand.RedirectActivation, hutaoArgs);
        clientStream.WritePacket(PrivateNamedPipe.Version, PipePacketType.SessionTermination, PipePacketCommand.None);
        clientStream.Flush();

        return true;
    }

    public void Dispose()
    {
        clientStream.Dispose();
    }

    private static void WaitPreviousProcessExit(ElevationStatusResponse response)
    {
        if (Process.GetProcessById(response.ProcessId) is { HasExited: false } process)
        {
            process.WaitForExit();
        }

        SpinWait.SpinUntil(() =>
        {
            try
            {
                _ = Process.GetProcessById(response.ProcessId);
                return false;
            }
            catch (ArgumentException)
            {
                return true;
            }
        });
    }
}