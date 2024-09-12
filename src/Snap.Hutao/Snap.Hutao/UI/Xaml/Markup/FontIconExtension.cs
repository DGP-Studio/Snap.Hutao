// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;

namespace Snap.Hutao.UI.Xaml.Markup;

/// <summary>
/// Custom <see cref="MarkupExtension"/> which can provide <see cref="FontIcon"/> values.
/// </summary>
[HighQuality]
[MarkupExtensionReturnType(ReturnType = typeof(FontIcon))]
internal sealed partial class FontIconExtension : MarkupExtension
{
    /// <summary>
    /// Gets or sets the <see cref="string"/> value representing the icon to display.
    /// </summary>
    public string Glyph { get; set; } = default!;

    public double FontSize { get; set; } = 12;

    /// <inheritdoc/>
    protected override object ProvideValue()
    {
        return new FontIcon()
        {
            Glyph = Glyph,
            FontSize = FontSize,
        };
    }
}