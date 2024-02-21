// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI.Animations;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Snap.Hutao.Control.Animation;
using Snap.Hutao.Control.Theme;
using Snap.Hutao.Service.BackgroundImage;
using Snap.Hutao.Service.Navigation;
using Snap.Hutao.View.Page;
using Snap.Hutao.ViewModel;

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
        DataContext = Ioc.Default.GetRequiredService<MainViewModel>();
        InitializeComponent();

        IServiceProvider serviceProvider = Ioc.Default;

        navigationService = serviceProvider.GetRequiredService<INavigationService>();
        if (navigationService is INavigationInitialization navigationInitialization)
        {
            navigationInitialization.Initialize(NavView, ContentFrame);
        }

        navigationService.Navigate<AnnouncementPage>(INavigationAwaiter.Default, true);
    }
}