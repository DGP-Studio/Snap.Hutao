// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Snap.Hutao.UI.Xaml.Control;

namespace Snap.Hutao.UI.Xaml.Data.Converter.Specialized;

/// <summary>
/// 条件转换器
/// </summary>
[DependencyProperty("ListValue", typeof(object))]
[DependencyProperty("GridValue", typeof(object))]
internal sealed partial class LayoutSwitchModeConverter : DependencyValueConverter<string, object>
{
    /// <inheritdoc/>
    public override object Convert(string from)
    {
        return from switch
        {
            LayoutSwitch.List => ListValue,
            LayoutSwitch.Grid => GridValue,
            _ => default!,
        };
    }
}