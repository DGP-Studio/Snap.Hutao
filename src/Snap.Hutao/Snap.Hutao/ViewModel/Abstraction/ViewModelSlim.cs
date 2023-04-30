// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Service.Navigation;

namespace Snap.Hutao.ViewModel.Abstraction;

/// <summary>
/// 简化的视图模型抽象类
/// </summary>
/// <typeparam name="TPage">页面类型</typeparam>
internal abstract class ViewModelSlim<TPage> : ObservableObject
    where TPage : Page
{
    private bool isInitialized;

    /// <summary>
    /// 构造一个新的简化的视图模型抽象类
    /// </summary>
    /// <param name="serviceProvider">服务提供器</param>
    public ViewModelSlim(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;

        OpenUICommand = new AsyncRelayCommand(OpenUIAsync);
        NavigateCommand = new RelayCommand(Navigate);
    }

    /// <summary>
    /// 是否初始化完成
    /// </summary>
    public bool IsInitialized { get => isInitialized; set => SetProperty(ref isInitialized, value); }

    /// <summary>
    /// 打开页面命令
    /// </summary>
    public ICommand OpenUICommand { get; }

    /// <summary>
    /// 导航命令
    /// </summary>
    public ICommand NavigateCommand { get; }

    /// <summary>
    /// 服务提供器
    /// </summary>
    protected IServiceProvider ServiceProvider { get; }

    /// <summary>
    /// 打开界面执行
    /// </summary>
    /// <returns>任务</returns>
    protected virtual Task OpenUIAsync()
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// 导航到指定的页面类型
    /// </summary>
    protected virtual void Navigate()
    {
        INavigationService navigationService = ServiceProvider.GetRequiredService<INavigationService>();
        navigationService.Navigate<TPage>(INavigationAwaiter.Default, true);
    }
}