// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Core;

namespace Snap.Hutao.View;

/// <summary>
/// 用户视图
/// </summary>
public sealed partial class UserView : UserControl
{
    private static readonly DependencyProperty IsExpandedProperty = Property<UserView>.Depend(nameof(IsExpanded), true);

    /// <summary>
    /// 构造一个新的用户视图
    /// </summary>
    public UserView()
    {
        InitializeComponent();
    }

    /// <summary>
    /// 当前用户控件是否处于展开状态
    /// </summary>
    public bool IsExpanded
    {
        get => (bool)GetValue(IsExpandedProperty);
        set => SetValue(IsExpandedProperty, value);
    }
}
