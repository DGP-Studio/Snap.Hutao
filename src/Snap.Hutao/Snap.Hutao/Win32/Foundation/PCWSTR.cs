// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace Snap.Hutao.Win32.Foundation;

internal readonly partial struct PCWSTR
{
    public readonly unsafe char* Value;

    public static unsafe implicit operator PCWSTR(char* value)
    {
        return *(PCWSTR*)&value;
    }

    public static unsafe implicit operator char*(PCWSTR value)
    {
        return *(char**)&value;
    }
}

#if DEBUG
[DebuggerDisplay("{DebuggerDisplay,nq}")]
internal readonly partial struct PCWSTR
{
    public unsafe string DebuggerDisplay
    {
        get => MemoryMarshal.CreateReadOnlySpanFromNullTerminated(Value).ToString();
    }
}
#endif