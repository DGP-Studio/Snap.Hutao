// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Win32.Foundation;

internal readonly struct PCSTR
{
    public readonly unsafe byte* Value;

    public static unsafe implicit operator PCSTR(byte* value)
    {
        return *(PCSTR*)&value;
    }

    public static unsafe implicit operator byte*(PCSTR value)
    {
        return *(byte**)&value;
    }
}