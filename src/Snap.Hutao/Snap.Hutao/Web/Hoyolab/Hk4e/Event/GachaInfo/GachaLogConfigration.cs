// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Hoyolab.Takumi.Binding;
using Snap.Hutao.Web.Request.QueryString;

namespace Snap.Hutao.Web.Hoyolab.Hk4e.Event.GachaInfo;

/// <summary>
/// 祈愿记录请求配置
/// </summary>
public struct GachaLogConfigration
{
    /// <summary>
    /// 尺寸
    /// </summary>
    public const int Size = 20;

    /// <summary>
    /// Below keys are required:
    /// authkey_ver
    /// auth_appid
    /// authkey
    /// sign_type
    /// Below keys used as control:
    /// lang
    /// gacha_type
    /// size
    /// end_id
    /// </summary>
    private readonly QueryString innerQuery;

    /// <summary>
    /// 构造一个新的祈愿记录请求配置
    /// </summary>
    /// <param name="query">原始查询字符串</param>
    /// <param name="type">祈愿类型</param>
    /// <param name="endId">终止Id</param>
    public GachaLogConfigration(string query, GachaConfigType type, long endId = 0L)
    {
        innerQuery = QueryString.Parse(query);

        innerQuery.Set("lang", "zh-cn");
        innerQuery.Set("gacha_type", (int)type);
        innerQuery.Set("size", Size);

        EndId = endId;
    }

    /// <summary>
    /// 是否为国际服
    /// </summary>
    public bool IsOversea { get; set; }

    /// <summary>
    /// 结束Id
    /// 控制API返回的分页
    /// 米哈游使用了 keyset pagination 来实现这一目标
    /// https://learn.microsoft.com/en-us/ef/core/querying/pagination#keyset-pagination
    /// </summary>
    public long EndId
    {
        get => long.Parse(innerQuery["end_id"]);
        set => innerQuery.Set("end_id", value);
    }

    /// <summary>
    /// 转换到查询字符串
    /// </summary>
    /// <param name="genAuthKeyData">生成信息</param>
    /// <param name="gameAuthKey">验证包装</param>
    /// <returns>查询</returns>
    public static string AsQuery(GenAuthKeyData genAuthKeyData, GameAuthKey gameAuthKey)
    {
        QueryString queryString = new();
        queryString.Set("auth_appid", genAuthKeyData.AuthAppId);
        queryString.Set("authkey", Uri.EscapeDataString(gameAuthKey.AuthKey));
        queryString.Set("authkey_ver", gameAuthKey.AuthKeyVersion);
        queryString.Set("sign_type", gameAuthKey.SignType);

        return queryString.ToString();
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