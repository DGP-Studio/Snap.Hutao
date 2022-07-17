// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Snap.Hutao.Core;

namespace Snap.Hutao.Control;

/// <summary>
/// 绑定探针
/// 用于处理特定情况下需要穿透数据上下文的工作
/// </summary>
public class BindingProxy : DependencyObject
{
    private static readonly DependencyProperty DataContextProperty = Property<BindingProxy>.Depend<object>(nameof(DataContext));

    /// <summary>
    /// 数据上下文
    /// </summary>
    public object? DataContext
    {
        get => GetValue(DataContextProperty);
        set => SetValue(DataContextProperty, value);
    }
}