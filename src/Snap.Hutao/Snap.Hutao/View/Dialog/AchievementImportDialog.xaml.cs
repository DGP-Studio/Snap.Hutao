// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Core;
using Snap.Hutao.Core.Threading;
using Snap.Hutao.Model.InterChange.Achievement;
using Snap.Hutao.Service.Achievement;

namespace Snap.Hutao.View.Dialog;

/// <summary>
/// 成就对话框
/// </summary>
public sealed partial class AchievementImportDialog : ContentDialog
{
    private static readonly DependencyProperty UIAFProperty = Property<AchievementImportDialog>.Depend(nameof(UIAF), default(UIAF));

    /// <summary>
    /// 构造一个新的成就对话框
    /// </summary>
    /// <param name="window">呈现的父窗口</param>
    /// <param name="uiaf">uiaf数据</param>
    public AchievementImportDialog(Window window, UIAF uiaf)
    {
        InitializeComponent();
        XamlRoot = window.Content.XamlRoot;
        UIAF = uiaf;
    }

    /// <summary>
    /// UIAF数据
    /// </summary>
    public UIAF UIAF
    {
        get => (UIAF)GetValue(UIAFProperty);
        set => SetValue(UIAFProperty, value);
    }

    /// <summary>
    /// 异步获取导入选项
    /// </summary>
    /// <returns>导入选项</returns>
    public async Task<Result<bool, ImportOption>> GetImportOptionAsync()
    {
        ContentDialogResult result = await ShowAsync();
        ImportOption option = (ImportOption)ImportModeSelector.SelectedIndex;

        return new Result<bool, ImportOption>(result == ContentDialogResult.Primary, option);
    }
}
