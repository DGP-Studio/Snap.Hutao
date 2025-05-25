// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Win32.Foundation;

// ReSharper disable InconsistentNaming
internal readonly struct NTSTATUS
{
#pragma warning disable CS0649
    public readonly int Value;
#pragma warning restore CS0649

    public override string ToString()
    {
        return $"0x{Value:X8}";
    }
}