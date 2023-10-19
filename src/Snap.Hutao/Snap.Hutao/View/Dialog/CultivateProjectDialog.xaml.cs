// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.User;

namespace Snap.Hutao.View.Dialog;

/// <summary>
/// 养成计划对话框
/// </summary>
[HighQuality]
[DependencyProperty("Text", typeof(string))]
[DependencyProperty("IsUidAttached", typeof(bool))]
internal sealed partial class CultivateProjectDialog : ContentDialog
{
    private readonly ITaskContext taskContext;

    /// <summary>
    /// 构造一个新的养成计划对话框
    /// </summary>
    /// <param name="serviceProvider">服务提供器</param>
    public CultivateProjectDialog(IServiceProvider serviceProvider)
    {
        InitializeComponent();

        taskContext = serviceProvider.GetRequiredService<ITaskContext>();
    }

    /// <summary>
    /// 创建一个新的，用户指定的计划
    /// </summary>
    /// <returns>计划</returns>
    public async ValueTask<ValueResult<bool, CultivateProject>> CreateProjectAsync()
    {
        await taskContext.SwitchToMainThreadAsync();
        ContentDialogResult result = await ShowAsync();
        if (result == ContentDialogResult.Primary)
        {
            string? uid = IsUidAttached
                ? Ioc.Default.GetRequiredService<IUserService>().Current?.SelectedUserGameRole?.GameUid
                : null;

            return new(true, CultivateProject.From(Text, uid));
        }

        return new(false, default!);
    }
}
