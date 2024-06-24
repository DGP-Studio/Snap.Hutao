// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI.Converters;
using Microsoft.UI.Xaml;

namespace Snap.Hutao.UI.Xaml.Data.Converter.Specialized;

internal sealed class BoolToGridLengthConverter : BoolToObjectConverter
{
    public BoolToGridLengthConverter()
    {
        TrueValue = new GridLength(1D, GridUnitType.Star);
        FalseValue = new GridLength(0D);
    }
}