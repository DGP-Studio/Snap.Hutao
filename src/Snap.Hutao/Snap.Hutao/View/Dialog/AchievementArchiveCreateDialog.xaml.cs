// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Snap.Hutao.View.Dialog;

/// <summary>
/// 成就存档创建对话框
/// </summary>
public sealed partial class AchievementArchiveCreateDialog : ContentDialog
{
    /// <summary>
    /// 构造一个新的成就存档创建对话框
    /// </summary>
    /// <param name="window">窗体</param>
    public AchievementArchiveCreateDialog(Window window)
    {
        InitializeComponent();
        XamlRoot = window.Content.XamlRoot;
    }

    /// <summary>
    /// 获取输入的字符串
    /// </summary>
    /// <returns>输入的结果</returns>
    public async Task<ValueResult<bool, string>> GetInputAsync()
    {
        ContentDialogResult result = await ShowAsync();
        string text = InputText.Text ?? string.Empty;

        return new(result == ContentDialogResult.Primary, text);
    }
}
