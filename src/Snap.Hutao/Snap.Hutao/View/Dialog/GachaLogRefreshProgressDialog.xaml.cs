// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Service.GachaLog;

namespace Snap.Hutao.View.Dialog;

/// <summary>
/// 祈愿记录刷新进度对话框
/// </summary>
[HighQuality]
[DependencyProperty("Status", typeof(GachaLogFetchStatus))]
internal sealed partial class GachaLogRefreshProgressDialog : ContentDialog
{
    /// <summary>
    /// 构造一个新的对话框
    /// </summary>
    /// <param name="serviceProvider">服务提供器</param>
    public GachaLogRefreshProgressDialog(IServiceProvider serviceProvider)
    {
        InitializeComponent();
    }

    /// <summary>
    /// 接收进度更新
    /// </summary>
    /// <param name="status">状态</param>
    public void OnReport(GachaLogFetchStatus status)
    {
        Status = status;
    }
}