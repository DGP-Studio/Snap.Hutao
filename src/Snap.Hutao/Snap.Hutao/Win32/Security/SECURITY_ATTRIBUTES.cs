// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;

namespace Snap.Hutao.Win32.Security;

[SuppressMessage("", "SA1307")]
internal struct SECURITY_ATTRIBUTES
{
    public uint nLength;
    public unsafe void* lpSecurityDescriptor;
    public BOOL bInheritHandle;
}