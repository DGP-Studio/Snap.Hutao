// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI.UI.Converters;
using Microsoft.UI.Xaml;

namespace Snap.Hutao.View.Converter;

/// <summary>
/// This class converts a collection size into a Visibility enumeration in reverse.
/// </summary>
public class EmptyCollectionToVisibilityRevertConverter : EmptyCollectionToObjectConverter
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EmptyCollectionToVisibilityRevertConverter"/> class.
    /// </summary>
    public EmptyCollectionToVisibilityRevertConverter()
    {
        EmptyValue = Visibility.Visible;
        NotEmptyValue = Visibility.Collapsed;
    }
}