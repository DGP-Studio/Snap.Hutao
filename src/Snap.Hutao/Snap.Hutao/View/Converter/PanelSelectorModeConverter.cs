// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Snap.Hutao.Control;
using Snap.Hutao.Control.Panel;

namespace Snap.Hutao.View.Converter;

/// <summary>
/// 条件转换器
/// </summary>
[DependencyProperty("ListValue", typeof(object))]
[DependencyProperty("GridValue", typeof(object))]
internal sealed partial class PanelSelectorModeConverter : DependencyValueConverter<string, object>
{
    /// <inheritdoc/>
    public override object Convert(string from)
    {
        return from switch
        {
            PanelSelector.List => ListValue,
            PanelSelector.Grid => GridValue,
            _ => default!,
        };
    }
}