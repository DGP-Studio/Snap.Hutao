// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Service.Navigation;

namespace Snap.Hutao.ViewModel.Abstraction;

/// <summary>
/// 简化的视图模型抽象类
/// </summary>
[ConstructorGenerated]
internal abstract partial class ViewModelSlim : ObservableObject
{
    private readonly IServiceProvider serviceProvider;
    private bool isInitialized;

    /// <summary>
    /// 是否初始化完成
    /// </summary>
    public bool IsInitialized { get => isInitialized; set => SetProperty(ref isInitialized, value); }

    /// <summary>
    /// 服务提供器
    /// </summary>
    protected IServiceProvider ServiceProvider { get => serviceProvider; }

    /// <summary>
    /// 打开界面执行
    /// </summary>
    /// <returns>任务</returns>
    [Command("OpenUICommand")]
    protected virtual Task OpenUIAsync()
    {
        return Task.CompletedTask;
    }
}

/// <summary>
/// 简化的视图模型抽象类
/// 可导航
/// </summary>
/// <typeparam name="TPage">要导航到的页面类型</typeparam>
[ConstructorGenerated(CallBaseConstructor = true)]
internal abstract partial class ViewModelSlim<TPage> : ViewModelSlim
    where TPage : Page
{
    /// <summary>
    /// 导航到指定的页面类型
    /// </summary>
    [Command("NavigateCommand")]
    protected virtual void Navigate()
    {
        INavigationService navigationService = ServiceProvider.GetRequiredService<INavigationService>();
        navigationService.Navigate<TPage>(INavigationAwaiter.Default, true);
    }
}