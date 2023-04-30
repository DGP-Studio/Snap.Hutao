// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.GachaLog.QueryProvider;

/// <summary>
/// 祈愿记录query
/// </summary>
[HighQuality]
internal readonly struct GachaLogQuery
{
    /// <summary>
    /// query
    /// </summary>
    public readonly string Query;

    /// <summary>
    /// 是否为国际服
    /// </summary>
    public readonly bool IsOversea;

    /// <summary>
    /// 消息
    /// </summary>
    public readonly string Message;

    /// <summary>
    /// 构造一个新的祈愿记录query
    /// </summary>
    /// <param name="query">query</param>
    public GachaLogQuery(string query)
    {
        Query = query;
        IsOversea = query.Contains("hoyoverse.com");
        Message = string.Empty;
    }

    private GachaLogQuery(string query, string message)
    {
        Query = query;
        Message = message;
    }

    public static implicit operator GachaLogQuery(string message)
    {
        return new(default!, message);
    }
}