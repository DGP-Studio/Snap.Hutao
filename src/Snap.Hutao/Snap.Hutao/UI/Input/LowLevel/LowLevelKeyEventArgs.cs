// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.UI.WindowsAndMessaging;

namespace Snap.Hutao.UI.Input.LowLevel;

internal sealed class LowLevelKeyEventArgs
{
    public LowLevelKeyEventArgs(KBDLLHOOKSTRUCT data)
    {
        Data = data;
    }

    public bool Handled { get; set; }

    public KBDLLHOOKSTRUCT Data { get; }
}