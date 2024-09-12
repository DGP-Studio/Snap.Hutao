﻿// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;

namespace Snap.Hutao.UI.Xaml.View.Dialog;

/// <summary>
/// 祈愿记录Url对话框
/// </summary>
[HighQuality]
[DependencyProperty("Text", typeof(string))]
internal sealed partial class GachaLogUrlDialog : ContentDialog
{
    private readonly ITaskContext taskContext;

    /// <summary>
    /// 初始化一个新的祈愿记录Url对话框
    /// </summary>
    /// <param name="serviceProvider">服务提供器</param>
    public GachaLogUrlDialog(IServiceProvider serviceProvider)
    {
        InitializeComponent();

        taskContext = serviceProvider.GetRequiredService<ITaskContext>();
    }

    /// <summary>
    /// 获取输入的Url
    /// </summary>
    /// <returns>输入的结果</returns>
    public async ValueTask<ValueResult<bool, string>> GetInputUrlAsync()
    {
        await taskContext.SwitchToMainThreadAsync();
        ContentDialogResult result = await ShowAsync();
        string url = Text.TrimEnd("#/log");

        return new(result == ContentDialogResult.Primary, url);
    }
}