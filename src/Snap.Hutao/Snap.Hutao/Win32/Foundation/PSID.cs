// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Security;

namespace Snap.Hutao.Win32.Foundation;

internal struct PSID
{
    public unsafe void* Value;

    public static unsafe implicit operator PSID(SID* value) => *(PSID*)&value;
}