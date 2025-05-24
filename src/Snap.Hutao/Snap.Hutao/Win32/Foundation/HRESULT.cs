// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Diagnostics;

namespace Snap.Hutao.Win32.Foundation;

// ReSharper disable InconsistentNaming
[SuppressMessage("", "SA1310")]
internal readonly partial struct HRESULT
{
#pragma warning disable CS0649
    public readonly int Value;
#pragma warning restore CS0649

    public static unsafe implicit operator int(HRESULT value)
    {
        return *(int*)&value;
    }

    public static unsafe implicit operator HRESULT(int value)
    {
        return *(HRESULT*)&value;
    }

    public override string ToString()
    {
        return $"0x{Value:X8}";
    }
}

#if DEBUG
[DebuggerDisplay("{DebuggerDisplay}")]
internal readonly partial struct HRESULT
{
    private string DebuggerDisplay { get => ToString(); }
}
#endif