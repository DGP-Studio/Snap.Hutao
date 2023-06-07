// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Snap.Hutao.Control;

namespace Snap.Hutao.View.Converter;

/// <summary>
/// 条件转换器
/// </summary>
internal sealed class PanelSelectorModeConverter : DependencyValueConverter<string, object>
{
    private static readonly DependencyProperty ListValueProperty = Property<PanelSelectorModeConverter>.Depend<object>(nameof(ListValue));
    private static readonly DependencyProperty GridValueProperty = Property<PanelSelectorModeConverter>.Depend<object>(nameof(GridValue));

    /// <summary>
    /// 列表值
    /// </summary>
    public object ListValue
    {
        get => GetValue(ListValueProperty);
        set => SetValue(ListValueProperty, value);
    }

    /// <summary>
    /// 网格值
    /// </summary>
    public object GridValue
    {
        get => GetValue(GridValueProperty);
        set => SetValue(GridValueProperty, value);
    }

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