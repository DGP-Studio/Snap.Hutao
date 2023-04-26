// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model;
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.ViewModel.SpiralAbyss;

/// <summary>
/// 角色视图
/// </summary>
[HighQuality]
internal class AvatarView : INameIconSide
{
    /// <summary>
    /// 构造一个新的角色视图
    /// </summary>
    /// <param name="metaAvatar">角色</param>
    public AvatarView(Model.Metadata.Avatar.Avatar metaAvatar)
    {
        Name = metaAvatar.Name;
        Icon = Model.Metadata.Converter.AvatarIconConverter.IconNameToUri(metaAvatar.Icon);
        SideIcon = Model.Metadata.Converter.AvatarIconConverter.IconNameToUri(metaAvatar.SideIcon);
        Quality = metaAvatar.Quality;
    }

    /// <summary>
    /// 构造一个新的角色视图
    /// </summary>
    /// <param name="avatarId">角色Id</param>
    /// <param name="idAvatarMap">Id角色映射</param>
    public AvatarView(in AvatarId avatarId, Dictionary<AvatarId, Model.Metadata.Avatar.Avatar> idAvatarMap)
        : this(idAvatarMap[avatarId])
    {
    }

    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// 图标
    /// </summary>
    public Uri Icon { get; }

    /// <summary>
    /// 侧面图标
    /// </summary>
    public Uri SideIcon { get; }

    /// <summary>
    /// 星级
    /// </summary>
    public ItemQuality Quality { get; }
}
