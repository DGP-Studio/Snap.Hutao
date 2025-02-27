// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Win32.Graphics.Gdi;

// RAIIFree: DeleteEnhMetaFile
// AlsoUsableFor: HGDIOBJ
// InvalidHandleValue: -1, 0
internal readonly struct HENHMETAFILE
{
    public readonly nint Value;
}