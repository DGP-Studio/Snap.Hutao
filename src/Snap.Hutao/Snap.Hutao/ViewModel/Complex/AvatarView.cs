// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Metadata.Avatar;
using Snap.Hutao.Model.Metadata.Converter;

namespace Snap.Hutao.ViewModel.Complex;

internal class AvatarView : CollocationView
{
    public AvatarView(Avatar avatar, double rate, double? lastRate = default)
        : base(rate, lastRate)
    {
        Name = avatar.Name;
        Icon = AvatarIconConverter.IconNameToUri(avatar.Icon);
        SideIcon = AvatarSideIconConverter.IconNameToUri(avatar.SideIcon);
        Quality = avatar.Quality;
    }

    public override string Name { get; }

    public override Uri Icon { get; }

    public override QualityType Quality { get; }

    public Uri SideIcon { get; }
}