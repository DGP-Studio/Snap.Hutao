// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;

namespace Snap.Hutao.UI.Xaml.Data.Converter;

[DependencyProperty<object>("VisibleValue")]
[DependencyProperty<object>("CollapsedValue")]
internal sealed partial class VisibilityToObjectConverter : DependencyValueConverter<Visibility, object?>
{
    public override object? Convert(Visibility from)
    {
        return from is Visibility.Visible ? VisibleValue : CollapsedValue;
    }
}