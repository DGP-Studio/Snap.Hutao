// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Model.Entity;

namespace Snap.Hutao.View.Dialog;

/// <summary>
/// 实时便笺通知设置对话框
/// </summary>
[HighQuality]
internal sealed partial class DailyNoteNotificationDialog : ContentDialog
{
    /// <summary>
    /// 构造一个新的实时便笺通知设置对话框
    /// </summary>
    /// <param name="serviceProvider">服务提供器</param>
    /// <param name="entry">实时便笺</param>
    public DailyNoteNotificationDialog(IServiceProvider serviceProvider, DailyNoteEntry entry)
    {
        InitializeComponent();

        DataContext = entry;
    }
}
