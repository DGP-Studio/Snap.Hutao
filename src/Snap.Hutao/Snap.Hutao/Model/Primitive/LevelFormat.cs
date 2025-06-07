// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.CompilerServices;

namespace Snap.Hutao.Model.Primitive;

internal static class LevelFormat
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Format(uint value)
    {
        return $"Lv. {value}";
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Format(uint value, uint extra)
    {
        return extra > 0 ? $"Lv. {value + extra} ({value} +{extra})" : $"Lv. {value}";
    }
}