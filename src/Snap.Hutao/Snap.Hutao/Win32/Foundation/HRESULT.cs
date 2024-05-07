// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Diagnostics;

namespace Snap.Hutao.Win32.Foundation;

[SuppressMessage("", "SA1310")]
internal readonly partial struct HRESULT
{
    public static readonly HRESULT E_FAIL = unchecked((int)0x80004005);

    public readonly int Value;

    public static unsafe implicit operator int(HRESULT value) => *(int*)&value;

    public static unsafe implicit operator HRESULT(int value) => *(HRESULT*)&value;

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