// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI.Converters;
using Microsoft.UI.Xaml;

namespace Snap.Hutao.View.Converter;

/// <summary>
/// This class converts a boolean value into a Visibility enumeration.
/// </summary>
[HighQuality]
internal sealed class BoolToVisibilityRevertConverter : BoolToObjectConverter
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BoolToVisibilityRevertConverter"/> class.
    /// </summary>
    public BoolToVisibilityRevertConverter()
    {
        TrueValue = Visibility.Collapsed;
        FalseValue = Visibility.Visible;
    }
}