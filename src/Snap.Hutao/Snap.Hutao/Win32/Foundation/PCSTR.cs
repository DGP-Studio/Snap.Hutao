// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace Snap.Hutao.Win32.Foundation;

// ReSharper disable InconsistentNaming
internal readonly partial struct PCSTR
{
#pragma warning disable CS0649
    public readonly unsafe byte* Value;
#pragma warning restore CS0649

    public static unsafe implicit operator PCSTR(byte* value)
    {
        return *(PCSTR*)&value;
    }

    public static unsafe implicit operator byte*(PCSTR value)
    {
        return *(byte**)&value;
    }
}

#if DEBUG
[DebuggerDisplay("{DebuggerDisplay,nq}")]
internal readonly partial struct PCSTR
{
    public unsafe string DebuggerDisplay
    {
        get => Encoding.UTF8.GetString(MemoryMarshal.CreateReadOnlySpanFromNullTerminated(Value));
    }
}
#endif