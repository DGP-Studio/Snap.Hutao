// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Win32.Foundation;

internal readonly struct PCWSTR
{
    public readonly unsafe char* Value;

    public static unsafe implicit operator PCWSTR(char* value) => *(PCWSTR*)&value;

    public static unsafe implicit operator char*(PCWSTR value) => *(char**)&value;
}