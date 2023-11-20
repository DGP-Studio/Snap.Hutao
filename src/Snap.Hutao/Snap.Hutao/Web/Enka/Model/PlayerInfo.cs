// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Web.Enka.Model;

/// <summary>
/// 玩家信息
/// </summary>
[HighQuality]
internal sealed class PlayerInfo
{
    /// <summary>
    /// 昵称
    /// Player Nickname
    /// </summary>
    [JsonPropertyName("nickname")]
    public string Nickname { get; set; } = default!;

    /// <summary>
    /// 等级
    /// </summary>
    [JsonPropertyName("level")]
    public uint Level { get; set; }

    /// <summary>
    /// 签名
    /// Profile Signature
    /// </summary>
    [JsonPropertyName("signature")]
    public string Signature { get; set; } = default!;

    /// <summary>
    /// 世界等级
    /// Player World Level
    /// </summary>
    [JsonPropertyName("worldLevel")]
    public uint WorldLevel { get; set; }

    /// <summary>
    /// 名片的Id
    /// Profile Namecard ID
    /// </summary>
    [JsonPropertyName("nameCardId")]
    public MaterialId NameCardId { get; set; }

    /// <summary>
    /// 完成的成就个数
    /// Number of Completed Achievements
    /// </summary>
    [JsonPropertyName("finishAchievementNum")]
    public uint FinishAchievementNum { get; set; }

    /// <summary>
    /// 深渊层数
    /// Abyss Floor
    /// </summary>
    [JsonPropertyName("towerFloorIndex")]
    public uint TowerFloorIndex { get; set; }

    /// <summary>
    /// 深渊间数
    /// Abyss Floor's Level
    /// </summary>
    [JsonPropertyName("towerLevelIndex")]
    public uint TowerLevelIndex { get; set; }

    /// <summary>
    /// 展示的角色信息
    /// List of Character IDs and Levels
    /// </summary>
    [JsonPropertyName("showAvatarInfoList")]
    public List<ShowAvatarInfo> ShowAvatarInfoList { get; set; } = default!;

    /// <summary>
    /// 展示的名片信息
    /// List of Namecard IDs
    /// </summary>
    [JsonPropertyName("showNameCardIdList")]
    public List<MaterialId> ShowNameCardIdList { get; set; } = default!;

    /// <summary>
    /// 头像信息
    /// Character ID of Profile Picture
    /// </summary>
    [JsonPropertyName("profilePicture")]
    public ProfilePicture ProfilePicture { get; set; } = default!;
}