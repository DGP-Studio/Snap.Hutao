// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Enka.Model;

/// <summary>
/// 档案头像
/// </summary>
public class ProfilePicture
{
    /// <summary>
    /// 使用的角色Id
    /// </summary>
    [JsonPropertyName("avatarId")]
    public int AvatarId { get; set; }
}