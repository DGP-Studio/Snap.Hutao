// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;

namespace Snap.Hutao.UI.Xaml.Markup;

[MarkupExtensionReturnType(ReturnType = typeof(BitmapIcon))]
internal sealed partial class BitmapIconExtension : MarkupExtension
{
    public Uri Source { get; set; } = default!;

    public bool ShowAsMonochrome { get; set; }

    protected override object ProvideValue()
    {
        return new BitmapIcon
        {
            ShowAsMonochrome = ShowAsMonochrome,
            UriSource = Source,
        };
    }
}