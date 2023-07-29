// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Snap.Hutao.Control;

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
            "List" => ListValue,
            "Grid" => GridValue,
            _ => default!,
        };
    }
}