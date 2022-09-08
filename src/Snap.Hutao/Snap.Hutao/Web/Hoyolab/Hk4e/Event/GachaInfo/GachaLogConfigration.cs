// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Request.QueryString;

namespace Snap.Hutao.Web.Hoyolab.Hk4e.Event.GachaInfo;

/// <summary>
/// 祈愿记录请求配置
/// </summary>
public struct GachaLogConfigration
{
    private readonly QueryString innerQuery;

    /// <summary>
    /// 构造一个新的祈愿记录请求配置
    /// </summary>
    /// <param name="query">原始查询字符串</param>
    /// <param name="type">祈愿类型</param>
    /// <param name="endId">终止Id</param>
    public GachaLogConfigration(string query, GachaConfigType type, ulong endId = 0UL)
    {
        innerQuery = QueryString.Parse(query);
        innerQuery.Set("lang", "zh-cn");

        Size = 20;
        Type = type;
        EndId = endId;
    }

    /// <summary>
    /// 尺寸
    /// </summary>
    public int Size
    {
        set => innerQuery.Set("size", value);
    }

    /// <summary>
    /// 类型
    /// </summary>
    public GachaConfigType Type
    {
        set => innerQuery.Set("gacha_type", (int)value);
    }

    /// <summary>
    /// 结束Id
    /// </summary>
    public ulong EndId
    {
        set => innerQuery.Set("end_id", value);
    }

    /// <summary>
    /// 转换到查询字符串
    /// </summary>
    /// <returns>匹配的查询字符串</returns>
    public string AsQuery()
    {
        return innerQuery.ToString();
    }
}