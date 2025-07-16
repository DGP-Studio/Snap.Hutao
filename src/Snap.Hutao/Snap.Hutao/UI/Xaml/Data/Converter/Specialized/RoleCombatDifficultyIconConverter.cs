// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;

namespace Snap.Hutao.UI.Xaml.Data.Converter.Specialized;

internal sealed partial class RoleCombatDifficultyIconConverter : ValueConverter<RoleCombatDifficultyLevel, Uri>
{
    public override Uri Convert(RoleCombatDifficultyLevel from)
    {
        return $"ms-appx:///Resource/Icon/UI_RoleCombat_Medal_S_{from:D}.png".ToUri();
    }
}