// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;

namespace Snap.Hutao.View.Dialog;

/// <summary>
/// 添加用户对话框
/// </summary>
[HighQuality]
internal sealed partial class UserDialog : ContentDialog
{
    private readonly ITaskContext taskContext;

    /// <summary>
    /// 构造一个新的添加用户对话框
    /// </summary>
    /// <param name="serviceProvider">服务提供器</param>
    public UserDialog(IServiceProvider serviceProvider)
    {
        InitializeComponent();
        XamlRoot = serviceProvider.GetRequiredService<MainWindow>().Content.XamlRoot;

        taskContext = serviceProvider.GetRequiredService<ITaskContext>();
    }

    /// <summary>
    /// 获取输入的Cookie
    /// </summary>
    /// <returns>输入的结果</returns>
    public async Task<ValueResult<bool, string>> GetInputCookieAsync()
    {
        await taskContext.SwitchToMainThreadAsync();
        ContentDialogResult result = await ShowAsync();
        string cookie = InputText.Text;

        return new(result == ContentDialogResult.Primary, cookie);
    }

    private void InputTextChanged(object sender, TextChangedEventArgs e)
    {
        IsPrimaryButtonEnabled = !string.IsNullOrEmpty(InputText.Text);
    }
}
