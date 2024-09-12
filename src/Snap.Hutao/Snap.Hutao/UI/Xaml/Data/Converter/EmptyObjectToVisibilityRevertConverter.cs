﻿// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI.Converters;
using Microsoft.UI.Xaml;

namespace Snap.Hutao.UI.Xaml.Data.Converter;

/// <summary>
/// This class converts a object? value into a Visibility enumeration in reverse.
/// </summary>
[HighQuality]
internal sealed partial class EmptyObjectToVisibilityRevertConverter : EmptyObjectToObjectConverter
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EmptyObjectToVisibilityRevertConverter"/> class.
    /// </summary>
    public EmptyObjectToVisibilityRevertConverter()
    {
        EmptyValue = Visibility.Visible;
        NotEmptyValue = Visibility.Collapsed;
    }
}
