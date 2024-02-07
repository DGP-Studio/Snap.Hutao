// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.IO.Pipes;

namespace Snap.Hutao.Core.LifeCycle.InterProcess;

internal static class NamedPipeClientStreamExtension
{
    public static bool TryConnectOnce(this NamedPipeClientStream clientStream)
    {
        try
        {
            clientStream.Connect(TimeSpan.Zero);
            return true;
        }
        catch (TimeoutException)
        {
            return false;
        }
    }
}
