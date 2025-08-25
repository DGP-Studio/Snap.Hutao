// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.UI.Xaml.Control;

namespace Snap.Hutao.UI.Xaml.Data.Converter.Specialized;

[DependencyProperty<object>("ListValue")]
[DependencyProperty<object>("GridValue")]
internal sealed partial class LayoutSwitchModeConverter : DependencyValueConverter<string, object?>
{
    public override object? Convert(string from)
    {
        return from switch
        {
            LayoutSwitch.List => ListValue,
            LayoutSwitch.Grid => GridValue,
            _ => default,
        };
    }
}