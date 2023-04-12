// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Control;
using Snap.Hutao.Model.Binding;
using Snap.Hutao.Service.GachaLog;
using Snap.Hutao.View.Control;

namespace Snap.Hutao.View.Dialog;

/// <summary>
/// 祈愿记录刷新进度对话框
/// </summary>
[HighQuality]
internal sealed partial class GachaLogRefreshProgressDialog : ContentDialog
{
    private static readonly DependencyProperty StateProperty = Property<GachaLogRefreshProgressDialog>.Depend<GachaLogFetchState>(nameof(State));

    /// <summary>
    /// 构造一个新的对话框
    /// </summary>
    /// <param name="window">窗体</param>
    public GachaLogRefreshProgressDialog()
    {
        InitializeComponent();
        XamlRoot = Ioc.Default.GetRequiredService<MainWindow>().Content.XamlRoot;
    }

    /// <summary>
    /// 刷新状态
    /// </summary>
    public GachaLogFetchState State
    {
        get => (GachaLogFetchState)GetValue(StateProperty);
        set => SetValue(StateProperty, value);
    }

    /// <summary>
    /// 接收进度更新
    /// </summary>
    /// <param name="state">状态</param>
    public void OnReport(GachaLogFetchState state)
    {
        State = state;
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