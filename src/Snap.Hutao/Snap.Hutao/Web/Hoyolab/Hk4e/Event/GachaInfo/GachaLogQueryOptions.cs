// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.GachaLog.QueryProvider;
using System.Collections.Specialized;
using System.Web;

namespace Snap.Hutao.Web.Hoyolab.Hk4e.Event.GachaInfo;

/// <summary>
/// 祈愿记录请求配置
/// </summary>
[HighQuality]
internal struct GachaLogQueryOptions
{
    /// <summary>
    /// 尺寸
    /// </summary>
    public const int Size = 20;

    /// <summary>
    /// 是否为国际服
    /// </summary>
    public readonly bool IsOversea;

    /// <summary>
    /// 结束Id
    /// 控制API返回的分页
    /// 米哈游使用了 keyset pagination 来实现这一目标
    /// https://learn.microsoft.com/en-us/ef/core/querying/pagination#keyset-pagination
    /// </summary>
    public long EndId;

    public GachaConfigType Type;

    /// <summary>
    /// Keys required:
    /// authkey_ver
    /// auth_appid
    /// authkey
    /// sign_type
    /// Keys used as control:
    /// lang
    /// gacha_type
    /// size
    /// end_id
    /// </summary>
    private readonly NameValueCollection innerQuery;

    /// <summary>
    /// 构造一个新的祈愿记录请求配置
    /// </summary>
    /// <param name="query">原始查询字符串</param>
    /// <param name="queryType">祈愿类型</param>
    /// <param name="endId">终止Id</param>
    public GachaLogQueryOptions(in GachaLogQuery query, GachaConfigType queryType)
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