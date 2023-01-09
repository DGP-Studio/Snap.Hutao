// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Control;
using Snap.Hutao.Extension;
using Snap.Hutao.Model.Binding.Gacha.Abstraction;
using Snap.Hutao.Service.GachaLog;
using Snap.Hutao.View.Control;

namespace Snap.Hutao.View.Dialog;

/// <summary>
/// 祈愿记录刷新进度对话框
/// </summary>
public sealed partial class GachaLogRefreshProgressDialog : ContentDialog
{
    private static readonly DependencyProperty StateProperty = Property<GachaLogRefreshProgressDialog>.Depend<FetchState>(nameof(State));

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
    public FetchState State
    {
        get => (FetchState)GetValue(StateProperty);
        set => SetValue(StateProperty, value);
    }

    /// <summary>
    /// 接收进度更新
    /// </summary>
    /// <param name="state">状态</param>
    public void OnReport(FetchState state)
    {
        State = state;
        GachaItemsPresenter.Header = state.AuthKeyTimeout
            ? null
            : (object)$"正在获取 {state.ConfigType.GetDescription()}";

        // Binding not working here.
        GachaItemsPresenter.Items.Clear();
        foreach (ItemBase item in state.Items)
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