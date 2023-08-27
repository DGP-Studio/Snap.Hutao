// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Snap.Hutao.View.Control;

/// <summary>
/// 统计卡片
/// </summary>
[HighQuality]
[DependencyProperty("ShowUpPull", typeof(bool), true)]
internal sealed partial class StatisticsCard : UserControl
{
    /// <summary>
    /// 构造一个新的统计卡片
    /// </summary>
    public StatisticsCard()
    {
        InitializeComponent();
    }
}
