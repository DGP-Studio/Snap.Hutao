// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Control;
using Snap.Hutao.Model.InterChange.GachaLog;

namespace Snap.Hutao.View.Dialog;

/// <summary>
/// 祈愿记录导入对话框
/// </summary>
[HighQuality]
internal sealed partial class GachaLogImportDialog : ContentDialog
{
    private static readonly DependencyProperty UIGFProperty = Property<AchievementImportDialog>.Depend(nameof(UIGF), default(UIGF));

    private readonly ITaskContext taskContext;

    /// <summary>
    /// 构造一个新的祈愿记录导入对话框
    /// </summary>
    /// <param name="serviceProvider">服务提供器</param>
    /// <param name="uigf">uigf数据</param>
    public GachaLogImportDialog(IServiceProvider serviceProvider, UIGF uigf)
    {
        InitializeComponent();
        XamlRoot = serviceProvider.GetRequiredService<MainWindow>().Content.XamlRoot;

        taskContext = serviceProvider.GetRequiredService<ITaskContext>();
        UIGF = uigf;
    }

    /// <summary>
    /// UIAF数据
    /// </summary>
    public UIGF UIGF
    {
        get => (UIGF)GetValue(UIGFProperty);
        set => SetValue(UIGFProperty, value);
    }

    /// <summary>
    /// 异步获取导入选项
    /// </summary>
    /// <returns>是否导入</returns>
    public async Task<bool> GetShouldImportAsync()
    {
        await taskContext.SwitchToMainThreadAsync();
        return await ShowAsync() == ContentDialogResult.Primary;
    }
}