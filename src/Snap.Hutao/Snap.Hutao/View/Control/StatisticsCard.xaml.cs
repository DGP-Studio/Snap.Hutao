// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Control;

namespace Snap.Hutao.View.Control;

/// <summary>
/// 统计卡片
/// </summary>
public sealed partial class StatisticsCard : UserControl
{
    private static readonly DependencyProperty ShowUpPullProperty = Property<StatisticsCard>.Depend(nameof(ShowUpPull), true);

    /// <summary>
    /// 构造一个新的统计卡片
    /// </summary>
    public StatisticsCard()
    {
        InitializeComponent();
    }

    /// <summary>
    /// 显示Up抽数
    /// </summary>
    public bool ShowUpPull
    {
        get { return (bool)GetValue(ShowUpPullProperty); }
        set { SetValue(ShowUpPullProperty, value); }
    }
}
