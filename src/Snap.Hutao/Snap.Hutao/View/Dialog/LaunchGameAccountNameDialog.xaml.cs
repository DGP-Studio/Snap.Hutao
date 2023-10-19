// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;

namespace Snap.Hutao.View.Dialog;

/// <summary>
/// 游戏账号命名对话框
/// </summary>
[HighQuality]
[DependencyProperty("Text", typeof(string))]
internal sealed partial class LaunchGameAccountNameDialog : ContentDialog
{
    private readonly ITaskContext taskContext;

    /// <summary>
    /// 构造一个新的游戏账号命名对话框
    /// </summary>
    /// <param name="serviceProvider">服务提供器</param>
    public LaunchGameAccountNameDialog(IServiceProvider serviceProvider)
    {
        InitializeComponent();

        taskContext = serviceProvider.GetRequiredService<ITaskContext>();
    }

    /// <summary>
    /// 获取输入的Cookie
    /// </summary>
    /// <returns>输入的结果</returns>
    public async ValueTask<ValueResult<bool, string>> GetInputNameAsync()
    {
        await taskContext.SwitchToMainThreadAsync();
        ContentDialogResult result = await ShowAsync();
        return new(result == ContentDialogResult.Primary && (!string.IsNullOrEmpty(Text)), Text);
    }
}
