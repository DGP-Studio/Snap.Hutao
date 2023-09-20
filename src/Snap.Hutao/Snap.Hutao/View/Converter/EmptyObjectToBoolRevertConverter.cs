// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI.Converters;

namespace Snap.Hutao.View.Converter;

/// <summary>
/// This class converts a object? value into a boolean in reverse.
/// </summary>
[HighQuality]
internal sealed class EmptyObjectToBoolRevertConverter : EmptyObjectToObjectConverter
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EmptyObjectToVisibilityRevertConverter"/> class.
    /// </summary>
    public EmptyObjectToBoolRevertConverter()
    {
        EmptyValue = true;
        NotEmptyValue = false;
    }
}