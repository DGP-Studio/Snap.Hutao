// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model;
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Metadata.Avatar;
using Snap.Hutao.Service.Metadata.ContextAbstraction;
using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.HardChallenge;

namespace Snap.Hutao.ViewModel.HardChallenge;

internal class AvatarView : INameIconSide<Uri>
{
    protected AvatarView(Avatar metaAvatar)
    {
        Name = metaAvatar.Name;
        Icon = Model.Metadata.Converter.AvatarIconConverter.IconNameToUri(metaAvatar.Icon);
        SideIcon = Model.Metadata.Converter.AvatarIconConverter.IconNameToUri(metaAvatar.SideIcon);
        Quality = metaAvatar.Quality;
    }

    private AvatarView(HardChallengeAvatar avatar, Avatar metaAvatar)
        : this(metaAvatar)
    {
        ActivatedConstellationCount = avatar.Rank;
    }

    private AvatarView(HardChallengeSimpleAvatar avatar, Avatar metaAvatar)
        : this(metaAvatar)
    {
    }

    public string Name { get; }

    public Uri Icon { get; }

    public Uri SideIcon { get; }

    public QualityType Quality { get; }

    public int ActivatedConstellationCount { get; init; }

    public static AvatarView Create(Avatar metaAvatar)
    {
        return new(metaAvatar);
    }

    public static AvatarView Create(HardChallengeAvatar avatar, HardChallengeMetadataContext context)
    {
        return new(avatar, context.GetAvatar(avatar.AvatarId));
    }

    public static AvatarView Create(HardChallengeSimpleAvatar avatar, HardChallengeMetadataContext context)
    {
        return new(avatar, context.GetAvatar(avatar.AvatarId));
    }

    public static AvatarView Create(HardChallengeAvatar avatar, Avatar metaAvatar)
    {
        return new(avatar, metaAvatar);
    }
}