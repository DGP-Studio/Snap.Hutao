// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Bbs.User;

internal sealed class AuditInfo
{
    [JsonPropertyName("is_nickname_in_audit")]
    public bool IsNicknameInAudit { get; set; }

    [JsonPropertyName("nickname")]
    public string Nickname { get; set; } = default!;

    [JsonPropertyName("is_introduce_in_audit")]
    public bool IsIntroduceInAudit { get; set; }

    [JsonPropertyName("introduce")]
    public string Introduce { get; set; } = default!;

    [JsonPropertyName("nickname_status")]
    public int NicknameStatus { get; set; }
}