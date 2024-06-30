// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.GachaLog.QueryProvider;
using Snap.Hutao.Web.Hoyolab.Hk4e.Event.GachaInfo;
using System.Collections.Specialized;
using System.Web;

namespace Snap.Hutao.Service.GachaLog;

internal struct GachaLogTypedQueryOptions
{
    public const int Size = 20;

    public readonly bool IsOversea;
    public readonly GachaType Type;

    public long EndId;

    private readonly NameValueCollection innerQuery;

    public GachaLogTypedQueryOptions(in GachaLogQuery query, GachaType queryType)
    {
        IsOversea = query.IsOversea;

        // 对于每个类型我们需要单独创建
        // 对应类型的 GachaLogQueryOptions
        Type = queryType;
        innerQuery = HttpUtility.ParseQueryString(query.Query);
        innerQuery.Set("gacha_type", $"{queryType:D}");
        innerQuery.Set("size", $"{Size}");
    }

    public readonly string ToQueryString()
    {
        // Make the cached end id into query.
        innerQuery.Set("end_id", $"{EndId:D}");
        string? query = innerQuery.ToString();
        ArgumentException.ThrowIfNullOrEmpty(query);
        return query;
    }
}