// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Specialized;
using System.Web;

namespace Snap.Hutao.Web.Hoyolab;

internal static class PlayerUidExtension
{
    public static string ToRoleIdServerQueryString(this in PlayerUid playerUid)
    {
        NameValueCollection collection = HttpUtility.ParseQueryString(string.Empty);
        collection.Set("role_id", playerUid.Value);
        collection.Set("server", playerUid.Region.Value);

        string? query = collection.ToString();
        ArgumentNullException.ThrowIfNull(query);
        return query;
    }

    public static string ToUidRegionQueryString(this in PlayerUid playerUid)
    {
        NameValueCollection collection = HttpUtility.ParseQueryString(string.Empty);
        collection.Set("uid", playerUid.Value);
        collection.Set("region", playerUid.Region.Value);

        string? query = collection.ToString();
        ArgumentNullException.ThrowIfNull(query);
        return query;
    }
}