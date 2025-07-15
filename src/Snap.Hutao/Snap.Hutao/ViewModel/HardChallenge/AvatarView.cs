// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model;
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Metadata.Avatar;
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

    private AvatarView(HardChallengeAvatar hardChallengeAvatar, Avatar metaAvatar)
        : this(metaAvatar)
    {
        ActivatedConstellationCount = hardChallengeAvatar.Rank;
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

    public static AvatarView Create(HardChallengeAvatar hardChallengeAvatar, Avatar metaAvatar)
    {
        return new(hardChallengeAvatar, metaAvatar);
    }
}