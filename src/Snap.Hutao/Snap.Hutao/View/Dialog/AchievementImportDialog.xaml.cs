// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Control;
using Snap.Hutao.Model.InterChange.Achievement;
using Snap.Hutao.Service.Achievement;

namespace Snap.Hutao.View.Dialog;

/// <summary>
/// 成就对话框
/// </summary>
[HighQuality]
internal sealed partial class AchievementImportDialog : ContentDialog
{
    private static readonly DependencyProperty UIAFProperty = Property<AchievementImportDialog>.Depend(nameof(UIAF), default(UIAF));

    private readonly ITaskContext taskContext;

    /// <summary>
    /// 构造一个新的成就对话框
    /// </summary>
    /// <param name="serviceProvider">服务提供器</param>
    /// <param name="uiaf">uiaf数据</param>
    public AchievementImportDialog(IServiceProvider serviceProvider, UIAF uiaf)
    {
        InitializeComponent();
        XamlRoot = serviceProvider.GetRequiredService<MainWindow>().Content.XamlRoot;

        taskContext = serviceProvider.GetRequiredService<ITaskContext>();
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
    public async Task<ValueResult<bool, ImportStrategy>> GetImportStrategyAsync()
    {
        await taskContext.SwitchToMainThreadAsync();
        ContentDialogResult result = await ShowAsync();
        ImportStrategy strategy = (ImportStrategy)ImportModeSelector.SelectedIndex;

        return new(result == ContentDialogResult.Primary, strategy);
    }
}
