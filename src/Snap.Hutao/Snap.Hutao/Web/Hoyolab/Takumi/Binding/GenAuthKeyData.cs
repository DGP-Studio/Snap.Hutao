// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Takumi.Binding;

/// <summary>
/// 验证密钥提交数据
/// im_css?
/// {"auth_appid":"im_ccs","game_biz":"bbs_cn","game_uid":0,"region":""}
/// </summary>
[HighQuality]
internal sealed class GenAuthKeyData
{
    /// <summary>
    /// 构造一个新的验证密钥提交数据
    /// </summary>
    /// <param name="authAppId">AppId</param>
    /// <param name="gameBiz">游戏代号</param>
    /// <param name="uid">uid</param>
    public GenAuthKeyData(string authAppId, string gameBiz, in PlayerUid uid)
    {
        AuthAppId = authAppId;
        GameBiz = gameBiz;
        GameUid = int.Parse(uid.Value);
        Region = uid.Region;
    }

    /// <summary>
    /// App Id
    /// </summary>
    [JsonPropertyName("auth_appid")]
    public string AuthAppId { get; set; } = default!;

    /// <summary>
    /// 游戏代号
    /// </summary>
    [JsonPropertyName("game_biz")]
    public string GameBiz { get; set; } = default!;

    /// <summary>
    /// Uid
    /// </summary>
    [JsonPropertyName("game_uid")]
    public int GameUid { get; set; }

    /// <summary>
    /// 区域
    /// </summary>
    [JsonPropertyName("region")]
    public string Region { get; set; } = default!;

    /// <summary>
    /// 创建为祈愿记录验证密钥提交数据
    /// </summary>
    /// <param name="uid">uid</param>
    /// <returns>验证密钥提交数据</returns>
    public static GenAuthKeyData CreateForWebViewGacha(in PlayerUid uid)
    {
        return new("webview_gacha", "hk4e_cn", uid);
    }
}