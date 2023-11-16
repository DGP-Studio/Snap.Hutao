// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Specialized;

namespace Snap.Hutao.Extension;

internal static partial class EnumerableExtension
{
    public static bool TryGetValue(this NameValueCollection collection, string name, [NotNullWhen(true)] out string? value)
    {
        if (collection.AllKeys.Contains(name))
        {
            if (collection.GetValues(name) is [string single])
            {
                value = single;
                return true;
            }
        }

        value = string.Empty;
        return false;
    }
}
