// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;

namespace Snap.Hutao.Win32.Security;

internal struct SID
{
    public byte Revision;
    public byte SubAuthorityCount;
    public SID_IDENTIFIER_AUTHORITY IdentifierAuthority;
    public FlexibleArray<uint> SubAuthority;
}