// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Request;
using System.Collections.Specialized;

namespace Snap.Hutao.Web.Hoyolab;

internal static class PlayerUidExtension
{
    public static string ToRoleIdServerQueryString(this in PlayerUid playerUid)
    {
        NameValueCollection collection = [];
        collection.Set("role_id", playerUid.Value);
        collection.Set("server", playerUid.Region);

        return collection.ToQueryString();
    }

    public static string ToUidRegionQueryString(this in PlayerUid playerUid)
    {
        NameValueCollection collection = [];
        collection.Set("uid", playerUid.Value);
        collection.Set("region", playerUid.Region);

        return collection.ToQueryString();
    }
}