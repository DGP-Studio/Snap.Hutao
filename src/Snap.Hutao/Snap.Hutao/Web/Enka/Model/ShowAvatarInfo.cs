// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Web.Enka.Model;

/// <summary>
/// 角色列表信息
/// </summary>
[HighQuality]
internal sealed class ShowAvatarInfo
{
    /// <summary>
    /// 角色Id
    /// Character ID
    /// </summary>
    [JsonPropertyName("avatarId")]
    public AvatarId AvatarId { get; set; }

    /// <summary>
    /// 角色等级
    /// Character Level
    /// </summary>
    [JsonPropertyName("level")]
    public Level Level { get; set; }

    /// <summary>
    /// 可能的皮肤Id
    /// </summary>
    [JsonPropertyName("costumeId")]
    public CostumeId? CostumeId { get; set; }
}