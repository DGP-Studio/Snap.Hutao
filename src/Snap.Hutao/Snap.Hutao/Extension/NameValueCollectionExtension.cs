// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Specialized;

namespace Snap.Hutao.Extension;

internal static class NameValueCollectionExtension
{
    public static bool TryGetSingleValue(this NameValueCollection collection, string name, [NotNullWhen(true)] out string? value)
    {
        if (collection.AllKeys.Contains(name) && collection.GetValues(name) is [{ } single])
        {
            value = single;
            return true;
        }

        value = string.Empty;
        return false;
    }
}
