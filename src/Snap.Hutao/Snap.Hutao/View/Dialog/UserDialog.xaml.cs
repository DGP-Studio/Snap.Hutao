// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Snap.Hutao.View.Dialog;

/// <summary>
/// 添加用户对话框
/// </summary>
[HighQuality]
[DependencyProperty("Text", typeof(string))]
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

        taskContext = serviceProvider.GetRequiredService<ITaskContext>();
    }

    /// <summary>
    /// 获取输入的Cookie
    /// </summary>
    /// <returns>输入的结果</returns>
    public async ValueTask<ValueResult<bool, string>> GetInputCookieAsync()
    {
        await taskContext.SwitchToMainThreadAsync();
        ContentDialogResult result = await ShowAsync();
        return new(result == ContentDialogResult.Primary && !string.IsNullOrEmpty(Text), Text);
    }
}
