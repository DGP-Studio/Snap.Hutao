// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Snap.Hutao.Core;

namespace Snap.Hutao.Control;

/// <summary>
/// 绑定探针
/// </summary>
public class BindingProxy : DependencyObject
{
    private static readonly DependencyProperty DataProperty = Property<BindingProxy>.Depend<object>(nameof(DataContext));

    /// <summary>
    /// 数据上下文
    /// </summary>
    public object? DataContext
    {
        get => GetValue(DataProperty);
        set => SetValue(DataProperty, value);
    }
}
