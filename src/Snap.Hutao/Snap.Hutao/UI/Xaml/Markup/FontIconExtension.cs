// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;

namespace Snap.Hutao.UI.Xaml.Markup;

[MarkupExtensionReturnType(ReturnType = typeof(FontIcon))]
internal sealed partial class FontIconExtension : MarkupExtension
{
    public string Glyph { get; set; } = default!;

    public double FontSize { get; set; } = 12;

    protected override object ProvideValue()
    {
        return new FontIcon
        {
            Glyph = Glyph,
            FontSize = FontSize,
        };
    }
}