// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Service.Navigation;
using Snap.Hutao.UI.Xaml.View.Page;
using Snap.Hutao.ViewModel;

namespace Snap.Hutao.UI.Xaml.View;

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
        IServiceProvider serviceProvider = Ioc.Default;

        this.InitializeDataContext<MainViewModel>(serviceProvider);

        InitializeComponent();

        (DataContext as MainViewModel)?.Initialize(new BackgroundImagePresenterAccessor(BackgroundImagePresenter));

        navigationService = serviceProvider.GetRequiredService<INavigationService>();
        if (navigationService is INavigationInitialization navigationInitialization)
        {
            navigationInitialization.Initialize(new NavigationViewAccessor(NavView, ContentFrame));
        }

        navigationService.Navigate<AnnouncementPage>(INavigationAwaiter.Default, true);
    }

    private class NavigationViewAccessor : INavigationViewAccessor
    {
        public NavigationViewAccessor(NavigationView navigationView, Frame frame)
        {
            NavigationView = navigationView;
            Frame = frame;
        }

        public NavigationView NavigationView { get; private set; }

        public Frame Frame { get; private set; }
    }

    private class BackgroundImagePresenterAccessor : IBackgroundImagePresenterAccessor
    {
        public BackgroundImagePresenterAccessor(Image backgroundImagePresenter)
        {
            BackgroundImagePresenter = backgroundImagePresenter;
        }

        public Image BackgroundImagePresenter { get; private set; }
    }
}