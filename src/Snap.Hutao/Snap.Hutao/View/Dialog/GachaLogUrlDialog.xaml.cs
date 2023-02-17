// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;

namespace Snap.Hutao.View.Dialog;

/// <summary>
/// 祈愿记录Url对话框
/// </summary>
[HighQuality]
internal sealed partial class GachaLogUrlDialog : ContentDialog
{
    /// <summary>
    /// 初始化一个新的祈愿记录Url对话框
    /// </summary>
    /// <param name="window">窗体</param>
    public GachaLogUrlDialog()
    {
        InitializeComponent();
        XamlRoot = Ioc.Default.GetRequiredService<MainWindow>().Content.XamlRoot;
    }

    /// <summary>
    /// 获取输入的Url
    /// </summary>
    /// <returns>输入的结果</returns>
    public async Task<ValueResult<bool, string>> GetInputUrlAsync()
    {
        await ThreadHelper.SwitchToMainThreadAsync();
        ContentDialogResult result = await ShowAsync();
        string url = InputText.Text;

        return new(result == ContentDialogResult.Primary, url);
    }
}
