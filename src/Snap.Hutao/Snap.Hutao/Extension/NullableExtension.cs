// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Extension;

internal static class NullableExtension
{
    public static bool TryGetValue<T>(this in T? nullable, out T value)
        where T : struct
    {
        if (nullable.HasValue)
        {
            value = nullable.Value;
            return true;
        }

        value = default;
        return false;
    }
}