// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Service.Abstraction;
using Snap.Hutao.Service.Navigation;
using Snap.Hutao.View.Page;

namespace Snap.Hutao.View;

/// <summary>
/// 主视图
/// </summary>
public sealed partial class MainView : UserControl
{
    private readonly INavigationService navigationService;
    private readonly IInfoBarService infoBarService;

    /// <summary>
    /// 构造一个新的主视图
    /// </summary>
    public MainView()
    {
        InitializeComponent();

        infoBarService = Ioc.Default.GetRequiredService<IInfoBarService>();
        infoBarService.Initialize(InfoBarStack);

        navigationService = Ioc.Default.GetRequiredService<INavigationService>();
        navigationService.Initialize(NavView, ContentFrame);

        navigationService.Navigate<AnnouncementPage>(INavigationAwaiter.Default, true);
    }
}