// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.Linq;

internal static class DictionaryLookupExtension
{
    public static ILookup<TKey, TValue> ToLookup<TKey, TValue>(this Dictionary<TKey, List<TValue>> dictionary)
        where TKey : notnull
    {
        return new DictionaryLookup<TKey, List<TValue>, TValue>(dictionary);
    }
}
