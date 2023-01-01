// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Model.Binding.SpiralAbyss;

/// <summary>
/// 角色
/// </summary>
public class Avatar
{
    /// <summary>
    /// 构造一个新的角色
    /// </summary>
    /// <param name="avatarId">角色Id</param>
    /// <param name="idAvatarMap">Id角色映射</param>
    public Avatar(AvatarId avatarId, Dictionary<AvatarId, Metadata.Avatar.Avatar> idAvatarMap)
    {
        System.Diagnostics.Debug.WriteLineIf(!idAvatarMap.ContainsKey(avatarId), avatarId.Value);
        Metadata.Avatar.Avatar metaAvatar = idAvatarMap[avatarId];
        Name = metaAvatar.Name;
        Icon = Metadata.Converter.AvatarIconConverter.IconNameToUri(metaAvatar.Icon);
        SideIcon = Metadata.Converter.AvatarIconConverter.IconNameToUri(metaAvatar.SideIcon);
        Quality = metaAvatar.Quality;
    }

    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; set; } = default!;

    /// <summary>
    /// 图标
    /// </summary>
    public Uri Icon { get; set; } = default!;

    /// <summary>
    /// 侧面图标
    /// </summary>
    public Uri SideIcon { get; set; } = default!;

    /// <summary>
    /// 星级
    /// </summary>
    public ItemQuality Quality { get; set; }
}
