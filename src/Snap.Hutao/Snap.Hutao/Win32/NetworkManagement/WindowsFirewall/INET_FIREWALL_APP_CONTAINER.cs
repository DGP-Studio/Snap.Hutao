// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.Security;

namespace Snap.Hutao.Win32.NetworkManagement.WindowsFirewall;

[SuppressMessage("", "SA1307")]
internal struct INET_FIREWALL_APP_CONTAINER
{
    public unsafe SID* appContainerSid;
    public unsafe SID* userSid;
    public PWSTR appContainerName;
    public PWSTR displayName;
    public PWSTR description;
    public INET_FIREWALL_AC_CAPABILITIES capabilities;
    public INET_FIREWALL_AC_BINARIES binaries;
    public PWSTR workingDirectory;
    public PWSTR packageFullName;
}