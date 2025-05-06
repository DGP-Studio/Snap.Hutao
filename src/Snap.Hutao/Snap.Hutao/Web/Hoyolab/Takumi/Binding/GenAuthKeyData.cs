// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Globalization;

namespace Snap.Hutao.Web.Hoyolab.Takumi.Binding;

internal sealed class GenAuthKeyData
{
    public GenAuthKeyData(string authAppId, string gameBiz, PlayerUid uid)
    {
        AuthAppId = authAppId;
        GameBiz = gameBiz;
        GameUid = int.Parse(uid.Value, CultureInfo.InvariantCulture);
        Region = uid.Region;
    }

    [JsonPropertyName("auth_appid")]
    public string AuthAppId { get; set; }

    [JsonPropertyName("game_biz")]
    public string GameBiz { get; set; }

    [JsonPropertyName("game_uid")]
    public int GameUid { get; set; }

    [JsonPropertyName("region")]
    public Region Region { get; set; }

    public static GenAuthKeyData CreateForWebViewGacha(PlayerUid uid)
    {
        return new("webview_gacha", "hk4e_cn", uid);
    }
}