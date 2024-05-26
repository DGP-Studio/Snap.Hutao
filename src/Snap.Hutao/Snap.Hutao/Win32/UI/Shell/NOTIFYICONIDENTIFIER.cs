// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;

namespace Snap.Hutao.Win32.UI.Shell;

[SuppressMessage("", "SA1307")]
internal struct NOTIFYICONIDENTIFIER
{
    public uint cbSize;
    public HWND hWnd;
    public uint uID;
    public Guid guidItem;
}