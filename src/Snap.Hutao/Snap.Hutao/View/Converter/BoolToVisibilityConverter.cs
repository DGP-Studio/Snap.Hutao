// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;

namespace Snap.Hutao.View.Converter;

/// <summary>
/// This class converts a boolean value into a Visibility enumeration.
/// </summary>
public class BoolToVisibilityConverter : BoolToObjectConverter
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BoolToVisibilityConverter"/> class.
    /// </summary>
    public BoolToVisibilityConverter()
    {
        TrueValue = Visibility.Visible;
        FalseValue = Visibility.Collapsed;
    }
}
