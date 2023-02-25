// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Control;
using Snap.Hutao.Model.Binding.BaseValue;

namespace Snap.Hutao.View.Control;

/// <summary>
/// 基础数值滑动条
/// </summary>
internal sealed partial class BaseValueSlider : UserControl
{
    private static readonly DependencyProperty BaseValueInfoProperty = Property<BaseValueSlider>.Depend<BaseValueInfo>(nameof(BaseValueInfo));
    private static readonly DependencyProperty IsPromoteVisibleProperty = Property<BaseValueSlider>.DependBoxed<bool>(nameof(IsPromoteVisible), BoxedValues.True);

    /// <summary>
    /// 构造一个新的基础数值滑动条
    /// </summary>
    public BaseValueSlider()
    {
        InitializeComponent();
    }

    /// <summary>
    /// 基础数值信息
    /// </summary>
    public BaseValueInfo BaseValueInfo
    {
        get => (BaseValueInfo)GetValue(BaseValueInfoProperty);
        set => SetValue(BaseValueInfoProperty, value);
    }

    /// <summary>
    /// 提升按钮是否可见
    /// </summary>
    public bool IsPromoteVisible
    {
        get { return (bool)GetValue(IsPromoteVisibleProperty); }
        set { SetValue(IsPromoteVisibleProperty, value); }
    }
}