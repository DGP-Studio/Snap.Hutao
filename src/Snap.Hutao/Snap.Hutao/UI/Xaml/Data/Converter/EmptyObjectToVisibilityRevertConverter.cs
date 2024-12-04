// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI.Converters;
using Microsoft.UI.Xaml;

namespace Snap.Hutao.UI.Xaml.Data.Converter;

internal sealed partial class EmptyObjectToVisibilityRevertConverter : EmptyObjectToObjectConverter
{
    public EmptyObjectToVisibilityRevertConverter()
    {
        EmptyValue = Visibility.Visible;
        NotEmptyValue = Visibility.Collapsed;
    }
}
