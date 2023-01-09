// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Snap.Hutao.View.Dialog;

/// <summary>
/// 游戏账号命名对话框
/// </summary>
public sealed partial class GameAccountNameDialog : ContentDialog
{
    /// <summary>
    /// 构造一个新的游戏账号命名对话框
    /// </summary>
    /// <param name="window">窗体</param>
    public GameAccountNameDialog()
    {
        InitializeComponent();
        XamlRoot = Ioc.Default.GetRequiredService<MainWindow>().Content.XamlRoot;
    }

    /// <summary>
    /// 获取输入的Cookie
    /// </summary>
    /// <returns>输入的结果</returns>
    public async Task<ValueResult<bool, string>> GetInputNameAsync()
    {
        await ThreadHelper.SwitchToMainThreadAsync();
        ContentDialogResult result = await ShowAsync();
        string text = InputText.Text;
        return new(result == ContentDialogResult.Primary && (!string.IsNullOrEmpty(text)), text);
    }
}
