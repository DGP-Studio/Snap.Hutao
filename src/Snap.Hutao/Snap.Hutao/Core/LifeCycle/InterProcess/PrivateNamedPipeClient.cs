// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Windows.AppLifecycle;
using Snap.Hutao.Core.LifeCycle.InterProcess.Model;
using System.Diagnostics;
using System.IO;
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

        try
        {
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
        catch (IOException)
        {
            // Pipe is broken.
            return false;
        }
    }

    public void Dispose()
    {
        clientStream.Dispose();
    }

    private static void WaitPreviousProcessExit(ElevationStatusResponse response)
    {
        Process process;
        try
        {
            process = Process.GetProcessById(response.ProcessId);
        }
        catch (ArgumentException)
        {
            // Process with an Id of ? is not running.
            return;
        }

        if (process is { HasExited: false })
        {
            try
            {
                process.WaitForExit();
            }
            catch (ArgumentException)
            {
                return;
            }
        }

        SpinWaitPolyfill.SpinUntil(response, static response =>
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