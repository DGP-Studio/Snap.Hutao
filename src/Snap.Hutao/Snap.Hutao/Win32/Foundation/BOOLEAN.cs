// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Win32.Foundation;

internal readonly struct BOOLEAN
{
    public readonly byte Value;

    public static unsafe implicit operator bool(BOOLEAN value)
    {
        return *(bool*)&value;
    }

    public static unsafe implicit operator BOOLEAN(bool value)
    {
        return *(BOOLEAN*)&value;
    }
}