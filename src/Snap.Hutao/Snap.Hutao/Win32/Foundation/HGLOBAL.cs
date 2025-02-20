// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Win32.Foundation;

// RAIIFree: GlobalFree
// InvalidHandleValue: -1, 0
internal readonly struct HGLOBAL
{
    public readonly nint Value;
}