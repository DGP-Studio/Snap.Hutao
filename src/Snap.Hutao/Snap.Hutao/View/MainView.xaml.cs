// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Core;
using Snap.Hutao.Core.Logging;
using Snap.Hutao.Core.Threading;
using Snap.Hutao.Service.Abstraction;
using Snap.Hutao.Service.Navigation;
using Snap.Hutao.View.Page;
using Windows.UI.ViewManagement;

namespace Snap.Hutao.View;

/// <summary>
/// 主视图
/// </summary>
public sealed partial class MainView : UserControl
{
    private readonly INavigationService navigationService;
    private readonly IInfoBarService infoBarService;
    private readonly UISettings uISettings;

    /// <summary>
    /// 构造一个新的主视图
    /// </summary>
    public MainView()
    {
        InitializeComponent();

        // 由于 PopupRoot 的 BUG, 需要手动响应主题色更改
        uISettings = Ioc.Default.GetRequiredService<UISettings>();
        uISettings.ColorValuesChanged += OnUISettingsColorValuesChanged;

        infoBarService = Ioc.Default.GetRequiredService<IInfoBarService>();
        infoBarService.Initialize(InfoBarStack);

        navigationService = Ioc.Default.GetRequiredService<INavigationService>();
        navigationService.Initialize(NavView, ContentFrame);

        navigationService.Navigate<AnnouncementPage>(INavigationAwaiter.Default, true);
    }

    private void OnUISettingsColorValuesChanged(UISettings sender, object args)
    {
        UpdateThemeAsync().SafeForget();
    }

    private async Task UpdateThemeAsync()
    {
        // It seems that UISettings.ColorValuesChanged
        // event can raise up on a background thread.
        await ThreadHelper.SwitchToMainThreadAsync();

        App current = Ioc.Default.GetRequiredService<App>();

        if (!ThemeHelper.Equals(current.RequestedTheme, RequestedTheme))
        {
            ILogger<MainView> logger = Ioc.Default.GetRequiredService<ILogger<MainView>>();
            logger.LogInformation(EventIds.CommonLog, "Element Theme [{element}] | App Theme [{app}]", RequestedTheme, current.RequestedTheme);

            // Update controls' theme which presents in the PopupRoot
            RequestedTheme = ThemeHelper.ApplicationToElement(current.RequestedTheme);
        }
    }
}