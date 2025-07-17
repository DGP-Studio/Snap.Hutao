// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;

namespace Snap.Hutao.UI.Xaml.Data.Converter.Specialized;

internal sealed partial class HardChallengeDifficultyIconConverter : ValueConverter<HardChallengeDifficultyLevel, Uri>
{
    public static Uri Convert(string iconName)
    {
        return $"ms-appx:///Resource/Icon/{iconName}.png".ToUri();
    }

    public override Uri Convert(HardChallengeDifficultyLevel from)
    {
        return $"ms-appx:///Resource/Icon/UI_LeyLineChallenge_Medal_{from:D}.png".ToUri();
    }
}