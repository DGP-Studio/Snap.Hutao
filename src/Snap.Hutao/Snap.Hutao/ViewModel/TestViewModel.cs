// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Input;
using Microsoft.Windows.AppLifecycle;
using Snap.Hutao.View.Dialog;

namespace Snap.Hutao.ViewModel;

/// <summary>
/// 测试视图模型
/// </summary>
[HighQuality]
[Injection(InjectAs.Scoped)]
internal sealed class TestViewModel : Abstraction.ViewModel
{
    private readonly ITaskContext taskContext;
    private readonly IServiceProvider serviceProvider;

    /// <summary>
    /// 构造一个新的测试视图模型
    /// </summary>
    /// <param name="serviceProvider">服务提供器</param>
    public TestViewModel(IServiceProvider serviceProvider)
    {
        taskContext = serviceProvider.GetRequiredService<ITaskContext>();
        this.serviceProvider = serviceProvider;

        ShowCommunityGameRecordDialogCommand = new AsyncRelayCommand(ShowCommunityGameRecordDialogAsync);
        ShowAdoptCalculatorDialogCommand = new AsyncRelayCommand(ShowAdoptCalculatorDialogAsync);
        RestartAppCommand = new RelayCommand<bool>(RestartApp);
    }

    /// <summary>
    /// 打开游戏社区记录对话框命令
    /// </summary>
    public ICommand ShowCommunityGameRecordDialogCommand { get; }

    /// <summary>
    /// 打开养成计算对话框命令
    /// </summary>
    public ICommand ShowAdoptCalculatorDialogCommand { get; }

    /// <summary>
    /// 重启命令
    /// </summary>
    public ICommand RestartAppCommand { get; }

    /// <inheritdoc/>
    protected override Task OpenUIAsync()
    {
        return Task.CompletedTask;
    }

    private async Task ShowCommunityGameRecordDialogAsync()
    {
        // ContentDialog must be created by main thread.
        await taskContext.SwitchToMainThreadAsync();
        await serviceProvider.CreateInstance<CommunityGameRecordDialog>().ShowAsync();
    }

    private async Task ShowAdoptCalculatorDialogAsync()
    {
        // ContentDialog must be created by main thread.
        await taskContext.SwitchToMainThreadAsync();
        await serviceProvider.CreateInstance<AdoptCalculatorDialog>().ShowAsync();
    }

    private void RestartApp(bool elevated)
    {
        AppInstance.Restart(string.Empty);
    }
}