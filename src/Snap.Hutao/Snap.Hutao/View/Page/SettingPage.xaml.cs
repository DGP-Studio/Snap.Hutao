// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Navigation;
using Snap.Hutao.Service.Navigation;
using Snap.Hutao.ViewModel;

namespace Snap.Hutao.View.Page;

/// <summary>
/// 设置页面
/// </summary>
public sealed partial class SettingPage : Microsoft.UI.Xaml.Controls.Page
{
    /// <summary>
    /// 构造新的设置页面
    /// </summary>
    public SettingPage()
    {
        DataContext = Ioc.Default.GetRequiredService<SettingViewModel>();
        InitializeComponent();
    }

    /// <inheritdoc/>
    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        if (e.Parameter is INavigationData data)
        {
            data.NotifyNavigationCompleted();
        }
    }
}