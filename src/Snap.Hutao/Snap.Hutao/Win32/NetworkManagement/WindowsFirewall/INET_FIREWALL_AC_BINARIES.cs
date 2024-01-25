// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;

namespace Snap.Hutao.Win32.NetworkManagement.WindowsFirewall;

[SuppressMessage("", "SA1307")]
internal struct INET_FIREWALL_AC_BINARIES
{
    public uint count;
    public unsafe PWSTR* binaries;
}