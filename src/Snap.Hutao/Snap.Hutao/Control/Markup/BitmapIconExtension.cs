// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;

namespace Snap.Hutao.Control.Markup;

/// <summary>
/// Custom <see cref="Markup"/> which can provide <see cref="BitmapIcon"/> values.
/// </summary>
[HighQuality]
[MarkupExtensionReturnType(ReturnType = typeof(BitmapIcon))]
internal sealed class BitmapIconExtension : MarkupExtension
{
    /// <summary>
    /// Gets or sets the <see cref="Uri"/> representing the image to display.
    /// </summary>
    public Uri Source { get; set; } = default!;

    /// <summary>
    /// Gets or sets a value indicating whether to display the icon as monochrome.
    /// </summary>
    public bool ShowAsMonochrome { get; set; }

    /// <inheritdoc/>
    protected override object ProvideValue()
    {
        return new BitmapIcon
        {
            ShowAsMonochrome = ShowAsMonochrome,
            UriSource = Source,
        };
    }
}