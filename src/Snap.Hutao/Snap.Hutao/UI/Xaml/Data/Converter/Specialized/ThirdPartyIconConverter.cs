// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;

namespace Snap.Hutao.UI.Xaml.Data.Converter.Specialized;

internal sealed partial class ThirdPartyIconConverter : ValueConverter<string, BitmapIcon>
{
    public override BitmapIcon Convert(string from)
    {
        Uri uri = $"ms-appx:///Resource/ThirdParty/{from}.png".ToUri();
        return new()
        {
            ShowAsMonochrome = false,
            UriSource = uri,
        };
    }
}