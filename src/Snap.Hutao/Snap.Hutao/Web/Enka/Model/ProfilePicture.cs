// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Web.Enka.Model;

/// <summary>
/// 档案头像
/// </summary>
[HighQuality]
internal sealed class ProfilePicture
{
    /// <summary>
    /// 使用的角色Id
    /// </summary>
    [JsonPropertyName("avatarId")]
    public AvatarId AvatarId { get; set; }

    /// <summary>
    /// 衣装Id
    /// </summary>
    [JsonPropertyName("costumeId")]
    public CostumeId? CostumeId { get; set; }
}