// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Model.InterChange.GachaLog;

namespace Snap.Hutao.View.Dialog;

/// <summary>
/// 祈愿记录导入对话框
/// </summary>
[HighQuality]
[DependencyProperty("UIGF", typeof(UIGF))]
internal sealed partial class GachaLogImportDialog : ContentDialog
{
    private readonly ITaskContext taskContext;

    /// <summary>
    /// 构造一个新的祈愿记录导入对话框
    /// </summary>
    /// <param name="serviceProvider">服务提供器</param>
    /// <param name="uigf">uigf数据</param>
    public GachaLogImportDialog(IServiceProvider serviceProvider, UIGF uigf)
    {
        InitializeComponent();

        taskContext = serviceProvider.GetRequiredService<ITaskContext>();
        UIGF = uigf;
    }

    /// <summary>
    /// 异步获取导入选项
    /// </summary>
    /// <returns>是否导入</returns>
    public async ValueTask<bool> GetShouldImportAsync()
    {
        await taskContext.SwitchToMainThreadAsync();
        return await ShowAsync() == ContentDialogResult.Primary;
    }
}