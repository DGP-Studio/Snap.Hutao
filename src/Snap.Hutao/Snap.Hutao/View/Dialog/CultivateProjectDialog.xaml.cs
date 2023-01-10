// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.User;

namespace Snap.Hutao.View.Dialog;

/// <summary>
/// 养成计划对话框
/// </summary>
public sealed partial class CultivateProjectDialog : ContentDialog
{
    /// <summary>
    /// 构造一个新的养成计划对话框
    /// </summary>
    /// <param name="window">窗体</param>
    public CultivateProjectDialog()
    {
        InitializeComponent();
        XamlRoot = Ioc.Default.GetRequiredService<MainWindow>().Content.XamlRoot;
    }

    /// <summary>
    /// 创建一个新的，用户指定的计划
    /// </summary>
    /// <returns>计划</returns>
    public async ValueTask<ValueResult<bool, CultivateProject>> CreateProjectAsync()
    {
        await ThreadHelper.SwitchToMainThreadAsync();
        ContentDialogResult result = await ShowAsync();
        if (result == ContentDialogResult.Primary)
        {
            string text = InputText.Text;
            string? uid = AttachUidBox.IsChecked == true
                ? Ioc.Default.GetRequiredService<IUserService>().Current?.SelectedUserGameRole?.GameUid
                : null;

            CultivateProject project = CultivateProject.Create(text, uid);
            return new(true, project);
        }

        return new(false, null!);
    }
}
