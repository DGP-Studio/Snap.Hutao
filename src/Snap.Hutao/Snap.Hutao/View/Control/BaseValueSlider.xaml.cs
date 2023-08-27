// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.ViewModel.Wiki;

namespace Snap.Hutao.View.Control;

/// <summary>
/// 基础数值滑动条
/// </summary>
[DependencyProperty("BaseValueInfo", typeof(BaseValueInfo))]
[DependencyProperty("IsPromoteVisible", typeof(bool), true)]
internal sealed partial class BaseValueSlider : UserControl
{
    /// <summary>
    /// 构造一个新的基础数值滑动条
    /// </summary>
    public BaseValueSlider()
    {
        InitializeComponent();
    }
}