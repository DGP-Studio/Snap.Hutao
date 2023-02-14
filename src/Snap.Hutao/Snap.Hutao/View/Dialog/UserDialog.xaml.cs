// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;

namespace Snap.Hutao.View.Dialog;

/// <summary>
/// 添加用户对话框
/// </summary>
internal sealed partial class UserDialog : ContentDialog
{
    /// <summary>
    /// 构造一个新的添加用户对话框
    /// </summary>
    /// <param name="window">呈现的父窗口</param>
    public UserDialog()
    {
        InitializeComponent();
        XamlRoot = Ioc.Default.GetRequiredService<MainWindow>().Content.XamlRoot;
    }

    /// <summary>
    /// 获取输入的Cookie
    /// </summary>
    /// <returns>输入的结果</returns>
    public async Task<ValueResult<bool, string>> GetInputCookieAsync()
    {
        await ThreadHelper.SwitchToMainThreadAsync();
        ContentDialogResult result = await ShowAsync();
        string cookie = InputText.Text;

        return new(result == ContentDialogResult.Primary, cookie);
    }

    private void InputTextChanged(object sender, TextChangedEventArgs e)
    {
        bool inputEmpty = string.IsNullOrEmpty(InputText.Text);

        (PrimaryButtonText, IsPrimaryButtonEnabled) = inputEmpty switch
        {
            true => ("请输入Cookie", false),
            false => ("确认", true),
        };
    }
}
