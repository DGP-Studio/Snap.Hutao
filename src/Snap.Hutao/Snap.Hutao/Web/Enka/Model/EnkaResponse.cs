// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Enka.Model;

/// <summary>
/// Enka API 响应
/// </summary>
[HighQuality]
internal sealed class EnkaResponse
{
    /// <summary>
    /// 玩家基础信息
    /// </summary>
    [JsonPropertyName("playerInfo")]
    public PlayerInfo? PlayerInfo { get; set; } = default!;

    /// <summary>
    /// 刷新剩余秒数
    /// 生存时间值
    /// </summary>
    [JsonPropertyName("ttl")]
    public int? TimeToLive { get; set; }

    /// <summary>
    /// 消息
    /// </summary>
    [JsonIgnore]
    public string Message { get; set; } = default!;
}