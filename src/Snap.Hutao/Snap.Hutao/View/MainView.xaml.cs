// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Service.Navigation;
using Snap.Hutao.View.Page;

namespace Snap.Hutao.View;

/// <summary>
/// 主视图
/// </summary>
[HighQuality]
internal sealed partial class MainView : UserControl
{
    private readonly INavigationService navigationService;

    /// <summary>
    /// 构造一个新的主视图
    /// </summary>
    public MainView()
    {
        InitializeComponent();

        IServiceProvider serviceProvider = Ioc.Default;

        navigationService = serviceProvider.GetRequiredService<INavigationService>();
        navigationService
            .As<INavigationInitialization>()?
            .Initialize(NavView, ContentFrame);

        navigationService.Navigate<AnnouncementPage>(INavigationAwaiter.Default, true);
    }
}