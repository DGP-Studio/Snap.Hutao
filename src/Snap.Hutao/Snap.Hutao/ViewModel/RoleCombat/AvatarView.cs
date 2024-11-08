// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model;
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Metadata.Avatar;
using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.RoleCombat;

namespace Snap.Hutao.ViewModel.RoleCombat;

internal class AvatarView : INameIconSide
{
    protected AvatarView(RoleCombatAvatar roleCombatAvatar, Avatar metaAvatar)
        : this(metaAvatar)
    {
        Type = roleCombatAvatar.AvatarType;
    }

    protected AvatarView(Avatar metaAvatar)
    {
        Name = metaAvatar.Name;
        Icon = Model.Metadata.Converter.AvatarIconConverter.IconNameToUri(metaAvatar.Icon);
        SideIcon = Model.Metadata.Converter.AvatarIconConverter.IconNameToUri(metaAvatar.SideIcon);
        Quality = metaAvatar.Quality;
    }

    public string Name { get; }

    public Uri Icon { get; }

    public Uri SideIcon { get; }

    public QualityType Quality { get; }

    public RoleCombatAvatarType Type { get; }

    public static AvatarView From(Avatar metaAvatar)
    {
        return new(metaAvatar);
    }

    public static AvatarView From(RoleCombatAvatar roleCombatAvatar, Avatar metaAvatar)
    {
        return new(roleCombatAvatar, metaAvatar);
    }
}
