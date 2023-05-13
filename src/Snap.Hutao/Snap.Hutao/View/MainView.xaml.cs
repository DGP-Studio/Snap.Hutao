// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Core.Setting;
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

        // this must be called before 'InitializeComponent'
        bool IsSwitchToStarRailTool = StaticResource.IsSwitchToStarRailTool();
        ContentSwitchPresenter.Value = IsSwitchToStarRailTool;

        IServiceProvider serviceProvider = Ioc.Default;

        navigationService = serviceProvider.GetRequiredService<INavigationService>();
        if (!IsSwitchToStarRailTool)
        {
            navigationService
                .As<INavigationInitialization>()?
                .Initialize(NavViewGenshin, ContentFrameGenshin);
        }
        else
        {
            navigationService
                .As<INavigationInitialization>()?
                .Initialize(NavViewStarRail, ContentFrameStarRail);
        }

        navigationService.Navigate<AnnouncementPage>(INavigationAwaiter.Default, true);
    }
}