// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using System.Runtime.InteropServices;

namespace Snap.Hutao.Win32.UI.Shell.Common;

[SuppressMessage("", "SA1307")]
[StructLayout(LayoutKind.Sequential, Pack = 1)]
internal struct SHITEMID
{
    public ushort cb;
    public FlexibleArray<byte> abID;
}