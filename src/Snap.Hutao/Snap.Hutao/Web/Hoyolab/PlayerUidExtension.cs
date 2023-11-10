// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Request.QueryString;

namespace Snap.Hutao.Web.Hoyolab;

internal static class PlayerUidExtension
{
    public static QueryString ToQueryString(this in PlayerUid playerUid)
    {
        QueryString queryString = new();
        queryString.Set("role_id", playerUid.Value);
        queryString.Set("server", playerUid.Region);

        return queryString;
    }
}