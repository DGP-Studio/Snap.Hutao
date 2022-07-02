// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Text.Json.Serialization;

namespace Snap.Hutao.Web.Enka.Model;

/// <summary>
/// 角色列表信息
/// </summary>
public class ShowAvatarInfo
{
    /// <summary>
    /// 角色Id
    /// Character ID
    /// </summary>
    [JsonPropertyName("avatarId")]
    public int AvatarId { get; set; }

    /// <summary>
    /// 角色等级
    /// Character Level
    /// </summary>
    [JsonPropertyName("level")]
    public int Level { get; set; }

    /// <summary>
    /// 可能的皮肤Id
    /// </summary>
    [JsonPropertyName("costumeId")]
    public int? CostumeId { get; set; }
}