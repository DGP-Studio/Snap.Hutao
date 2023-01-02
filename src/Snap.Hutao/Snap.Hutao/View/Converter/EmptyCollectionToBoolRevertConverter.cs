// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI.UI.Converters;
using Microsoft.UI.Xaml;

namespace Snap.Hutao.View.Converter;

/// <summary>
/// This class converts a collection size into a boolean value in reverse.
/// </summary>
public class EmptyCollectionToBoolRevertConverter : EmptyCollectionToObjectConverter
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EmptyCollectionToVisibilityConverter"/> class.
    /// </summary>
    public EmptyCollectionToBoolRevertConverter()
    {
        EmptyValue = true;
        NotEmptyValue = false;
    }
}