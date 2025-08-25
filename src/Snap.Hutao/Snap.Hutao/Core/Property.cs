// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core;

internal static class Property
{
    public static T Get<T>(this IProperty<T> property)
    {
        return property.Value;
    }

    public static T Set<T>(this IProperty<T> property, T value)
    {
        return property.Value = value;
    }
}