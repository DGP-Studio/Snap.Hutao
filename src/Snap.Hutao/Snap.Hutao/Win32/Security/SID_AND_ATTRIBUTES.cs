// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;

namespace Snap.Hutao.Win32.Security;

internal struct SID_AND_ATTRIBUTES
{
    public unsafe PSID Sid;
    public uint Attributes;
}