// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Request;
using System.Collections.Specialized;

namespace Snap.Hutao.Web.Hoyolab;

internal static class PlayerUidExtension
{
    public static string ToQueryString(this in PlayerUid playerUid)
    {
        NameValueCollection collection = [];
        collection.Set("role_id", playerUid.Value);
        collection.Set("server", playerUid.Region);

        return collection.ToQueryString();
    }
}