// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;

namespace Snap.Hutao.UI.Xaml.View.Card;

/// <summary>
/// 祈愿记录卡片
/// </summary>
internal sealed partial class GachaStatisticsCard : Button
{
    /// <summary>
    /// 构造一个新的祈愿记录卡片
    /// </summary>
    public GachaStatisticsCard()
    {
        this.InitializeDataContext<ViewModel.GachaLog.GachaLogViewModelSlim>();
        InitializeComponent();
    }
}
