// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Markup;
using System.Globalization;

namespace Snap.Hutao.Control.Markup;

/// <summary>
/// Xaml extension to return a <see cref="string"/> value from resource file associated with a resource key
/// </summary>
[HighQuality]
[MarkupExtensionReturnType(ReturnType = typeof(string))]
internal sealed class ResourceStringExtension : MarkupExtension
{
    /// <summary>
    /// Gets or sets associated ID from resource strings.
    /// </summary>
    public string? Name { get; set; }

    /// <inheritdoc/>
    protected override object ProvideValue()
    {
        return SH.ResourceManager.GetString(Name ?? string.Empty, CultureInfo.CurrentCulture) ?? Name ?? string.Empty;
    }
}