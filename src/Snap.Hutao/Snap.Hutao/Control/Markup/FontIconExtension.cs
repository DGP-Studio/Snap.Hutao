// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;

namespace Snap.Hutao.Control.Markup;

/// <summary>
/// Custom <see cref="Microsoft.UI.Xaml.Markup.MarkupExtension"/> which can provide <see cref="FontIcon"/> values.
/// </summary>
[MarkupExtensionReturnType(ReturnType = typeof(FontIcon))]
internal class FontIconExtension : MarkupExtension
{
    /// <summary>
    /// Gets or sets the <see cref="string"/> value representing the icon to display.
    /// </summary>
    public string Glyph { get; set; } = default!;

    /// <inheritdoc/>
    protected override object ProvideValue()
    {
        return new FontIcon()
        {
            Glyph = Glyph,
        };
    }
}
