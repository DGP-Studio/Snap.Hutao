// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;

namespace Snap.Hutao.Control.Markup;

/// <summary>
/// Custom <see cref="MarkupExtension"/> which can provide <see cref="FontIcon"/> values.
/// </summary>
[HighQuality]
[MarkupExtensionReturnType(ReturnType = typeof(FontIcon))]
internal sealed class FontIconExtension : MarkupExtension
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