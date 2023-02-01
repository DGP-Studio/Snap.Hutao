// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Markup;

namespace Snap.Hutao.Control.Markup;

/// <summary>
/// Xaml extension to return a <see cref="string"/> value from resource file associated with a resource key
/// </summary>
[MarkupExtensionReturnType(ReturnType = typeof(string))]
public sealed class ResourceStringExtension : MarkupExtension
{
    /// <summary>
    /// Gets or sets associated ID from resource strings.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets a string value from resource file associated with a resource key.
    /// </summary>
    /// <param name="name">Resource key name.</param>
    /// <returns>A string value from resource file associated with a resource key.</returns>
    public static string GetValue(string name)
    {
        // This function is needed to accomodate compiled function usage without second paramater,
        // which doesn't work with optional values.
        return SH.ResourceManager.GetString(name)!;
    }

    /// <inheritdoc/>
    protected override object ProvideValue()
    {
        return GetValue(Name ?? string.Empty);
    }
}