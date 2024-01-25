// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.InteropServices;
using Snap.Hutao.Win32.Foundation;

namespace Snap.Hutao.Win32.UI.Shell.Common;

[SuppressMessage("", "SA1307")]
[StructLayout(LayoutKind.Sequential, Pack = 1)]
internal struct SHITEMID
{
    public ushort cb;
    public FlexibleArray<byte> abID;
}