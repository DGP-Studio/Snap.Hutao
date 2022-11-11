// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Snap.Hutao.View.Dialog;

/// <summary>
/// 祈愿记录Url对话框
/// </summary>
public sealed partial class GachaLogUrlDialog : ContentDialog
{
    /// <summary>
    /// 初始化一个新的祈愿记录Url对话框
    /// </summary>
    /// <param name="window">窗体</param>
    public GachaLogUrlDialog(Window window)
    {
        InitializeComponent();
        XamlRoot = window.Content.XamlRoot;
    }

    /// <summary>
    /// 获取输入的Url
    /// </summary>
    /// <returns>输入的结果</returns>
    public async Task<ValueResult<bool, string>> GetInputUrlAsync()
    {
        ContentDialogResult result = await ShowAsync();
        string url = InputText.Text;

        return new(result == ContentDialogResult.Primary, url);
    }
}
