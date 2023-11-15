// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Snap.Hutao.SourceGeneration.Primitive;

internal static class AttributeDataExtension
{
    public static bool HasNamedArgumentWith<TValue>(this AttributeData data, string key, Func<TValue, bool> predicate)
    {
        return data.NamedArguments.Any(a => a.Key == key && predicate((TValue)a.Value.Value!));
    }

    public static bool TryGetNamedArgumentValue(this AttributeData data, string key, out TypedConstant value)
    {
        foreach (KeyValuePair<string, TypedConstant> pair in data.NamedArguments)
        {
            if (pair.Key == key)
            {
                value = pair.Value;
                return true;
            }
        }

        value = default;
        return false;
    }
}