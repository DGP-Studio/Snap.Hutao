// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;

namespace Snap.Hutao.UI.Xaml.Data.Converter;

[DependencyProperty("Value", typeof(string))]
internal sealed partial class SpecificStringToVisibilityConverter : DependencyValueConverter<string, Visibility>
{
    public override Visibility Convert(string from)
    {
        return string.Equals(from, Value, StringComparison.Ordinal) ? Visibility.Visible : Visibility.Collapsed;
    }
}