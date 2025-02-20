// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Win32.Graphics.Gdi;

// RAIIFree: DeleteObject
// AlsoUsableFor: HGDIOBJ
// InvalidHandleValue: -1, 0
internal readonly struct HBITMAP
{
    public readonly nint Value;
}