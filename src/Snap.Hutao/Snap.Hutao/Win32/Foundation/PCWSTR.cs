// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Win32.Foundation;

// ReSharper disable InconsistentNaming
internal readonly partial struct PCWSTR
{
#pragma warning disable CS0649
    public readonly unsafe char* Value;
#pragma warning restore CS0649

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
[global::System.Diagnostics.DebuggerDisplay("{DebuggerDisplay,nq}")]
internal readonly partial struct PCWSTR
{
    public unsafe string DebuggerDisplay
    {
        get => global::System.Runtime.InteropServices.MemoryMarshal.CreateReadOnlySpanFromNullTerminated(Value).ToString();
    }
}
#endif