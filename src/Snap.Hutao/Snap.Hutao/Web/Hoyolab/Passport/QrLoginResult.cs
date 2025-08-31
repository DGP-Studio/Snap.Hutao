// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Passport;

internal sealed class QrLoginResult
{
    [JsonPropertyName("status")]
    public string Status { get; set; } = default!;

    [JsonPropertyName("app_id")]
    public string AppId { get; set; } = default!;

    [JsonPropertyName("client_type")]
    public int ClientType { get; set; }

    [JsonPropertyName("created_at")]
    public string CreatedAt { get; set; } = default!;

    [JsonPropertyName("scanned_at")]
    public string ScannedAt { get; set; } = default!;

    [JsonPropertyName("tokens")]
    public List<TokenWrapper> Tokens { get; set; } = default!;

    [JsonPropertyName("user_info")]
    public UserInformation UserInfo { get; set; } = default!;

    [JsonPropertyName("realname_info")]
    public RealnameInfo RealnameInfo { get; set; } = default!;

    [JsonPropertyName("need_realperson")]
    public bool NeedRealperson { get; set; }

    [JsonPropertyName("ext")]
    public string Ext { get; set; } = default!;

    [JsonPropertyName("scan_game_biz")]
    public string ScanGameBiz { get; set; } = default!;
}