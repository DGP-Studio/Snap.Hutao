// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Control;
using Snap.Hutao.Model;
using Snap.Hutao.Service.GachaLog;
using Snap.Hutao.View.Control;

namespace Snap.Hutao.View.Dialog;

/// <summary>
/// 祈愿记录刷新进度对话框
/// </summary>
[HighQuality]
internal sealed partial class GachaLogRefreshProgressDialog : ContentDialog
{
    private static readonly DependencyProperty StatusProperty = Property<GachaLogRefreshProgressDialog>.Depend<GachaLogFetchStatus>(nameof(Status));

    /// <summary>
    /// 构造一个新的对话框
    /// </summary>
    /// <param name="serviceProvider">服务提供器</param>
    public GachaLogRefreshProgressDialog(IServiceProvider serviceProvider)
    {
        InitializeComponent();
        XamlRoot = serviceProvider.GetRequiredService<MainWindow>().Content.XamlRoot;
    }

    /// <summary>
    /// 刷新状态
    /// </summary>
    public GachaLogFetchStatus Status
    {
        get => (GachaLogFetchStatus)GetValue(StatusProperty);
        set => SetValue(StatusProperty, value);
    }

    /// <summary>
    /// 接收进度更新
    /// </summary>
    /// <param name="state">状态</param>
    public void OnReport(GachaLogFetchStatus state)
    {
        Status = state;

        // TODO: test new binding approach
        GachaItemsPresenter.Header = state.AuthKeyTimeout
            ? SH.ViewDialogGachaLogRefreshProgressAuthkeyTimeout
            : string.Format(SH.ViewDialogGachaLogRefreshProgressDescription, state.ConfigType.GetLocalizedDescription());

        // Binding not working here.
        GachaItemsPresenter.Items.Clear();

        // System.InvalidOperationException: Collection was modified; enumeration operation may not execute.
        foreach (Item item in state.Items.ToList())
        {
            GachaItemsPresenter.Items.Add(new ItemIcon
            {
                Width = 60,
                Height = 60,
                Quality = item.Quality,
                Icon = item.Icon,
                Badge = item.Badge,
            });
        }
    }
}