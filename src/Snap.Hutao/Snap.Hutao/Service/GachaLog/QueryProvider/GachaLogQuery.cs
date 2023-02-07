// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.GachaLog.QueryProvider;

/// <summary>
/// 祈愿记录query
/// </summary>
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
    /// <param name="isOversea">是否为国际服</param>
    public GachaLogQuery(string query, bool isOversea)
    {
        Query = query;
        IsOversea = isOversea;
        Message = string.Empty;
    }

    /// <summary>
    /// 构造一个新的失败的祈愿记录query
    /// </summary>
    /// <param name="message">失败原因</param>
    public GachaLogQuery(string message)
    {
        Message = message;
        Query = string.Empty;
    }

    public static implicit operator GachaLogQuery(string message)
    {
        return new(message);
    }
}