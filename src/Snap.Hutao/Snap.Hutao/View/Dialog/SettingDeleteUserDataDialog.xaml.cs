// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Windows.Win32.Foundation;

namespace Snap.Hutao.View.Dialog;

/// <summary>
/// 删除信息对话框
/// </summary>
internal sealed partial class SettingDeleteUserDataDialog : ContentDialog
{
    /// <summary>
    /// 构造一个新的删除信息对话框
    /// </summary>
    public SettingDeleteUserDataDialog()
    {
        InitializeComponent();
        XamlRoot = Ioc.Default.GetRequiredService<MainWindow>().Content.XamlRoot;
    }

    /// <summary>
    /// 获取点击的按钮
    /// </summary>
    /// <returns>点击的结果</returns>
    public async Task<bool> GetClickButtonResultAsync()
    {
        await ThreadHelper.SwitchToMainThreadAsync();
        ContentDialogResult result = await ShowAsync();
        return result == ContentDialogResult.Primary;
    }
}
