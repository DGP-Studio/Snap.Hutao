// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;

namespace Snap.Hutao.UI.Xaml.Data.Converter.Specialized;

internal sealed partial class ThirdPartyIconConverter : ValueConverter<string, BitmapIcon>
{
    public const string TwitterName = "X (Twitter)";

    public override BitmapIcon Convert(string from)
    {
        Uri uri = from switch
        {
            TwitterName => $"ms-appx:///Resource/ThirdParty/Twitter.png".ToUri(),
            _ => $"ms-appx:///Resource/ThirdParty/{from}.png".ToUri(),
        };

        return new()
        {
            ShowAsMonochrome = false,
            UriSource = uri,
        };
    }
}