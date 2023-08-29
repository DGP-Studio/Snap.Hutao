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
    /// 展示的角色详细信息列表
    /// 如果为 null 说明未打开角色橱窗
    /// </summary>
    [JsonPropertyName("avatarInfoList")]
    public List<AvatarInfo>? AvatarInfoList { get; set; } = default!;

    /// <summary>
    /// 刷新剩余秒数
    /// 生存时间值
    /// </summary>
    [JsonPropertyName("ttl")]
    public int? TimeToLive { get; set; }

    /// <summary>
    /// 此响应是否有效
    /// </summary>
    public bool IsValid
    {
        [MemberNotNullWhen(true, nameof(PlayerInfo), nameof(AvatarInfoList))]
        get => PlayerInfo is not null && AvatarInfoList is not null;
    }

    /// <summary>
    /// 消息
    /// </summary>
    [JsonIgnore]
    public string Message { get; set; } = default!;
}