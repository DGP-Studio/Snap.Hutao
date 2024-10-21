// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using System.IO.Pipes;

namespace Snap.Hutao.Core.LifeCycle.InterProcess.BetterGenshinImpact;

[Injection(InjectAs.Singleton)]
internal sealed partial class BetterGenshinImpactNamedPipeClient : IDisposable
{
    private readonly NamedPipeClientStream clientStream = new(".", "BetterGenshinImpact.NamedPipe", PipeDirection.InOut, PipeOptions.Asynchronous | PipeOptions.WriteThrough);

    public void Dispose()
    {
        clientStream.Dispose();
    }

    public bool TryStartCapture(HWND hwnd)
    {
        if (!clientStream.TryConnectOnce())
        {
            return false;
        }

        PipeRequest<long> startCaptureRequest = new() { Kind = PipeRequestKind.StartCapture, Data = hwnd };
        clientStream.WritePacketWithJsonContent(PrivateNamedPipe.Version, PipePacketType.Request, PipePacketCommand.SnapHutaoToBetterGenshinImpactRequest, startCaptureRequest);
        clientStream.WritePacket(PrivateNamedPipe.Version, PipePacketType.SessionTermination, PipePacketCommand.None);
        return true;
    }
}
