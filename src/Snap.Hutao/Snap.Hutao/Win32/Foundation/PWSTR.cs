// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Win32.Foundation;

internal readonly struct PWSTR
{
    public readonly unsafe char* Value;

    public static unsafe implicit operator PWSTR(char* value) => *(PWSTR*)&value;

    public static unsafe implicit operator char*(PWSTR value) => *(char**)&value;
}