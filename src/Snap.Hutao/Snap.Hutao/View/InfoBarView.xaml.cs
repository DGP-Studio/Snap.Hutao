// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Snap.Hutao.Control;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.Service.Notification;
using System.Collections.ObjectModel;

namespace Snap.Hutao.View;

/// <summary>
/// 信息条视图
/// </summary>
internal sealed partial class InfoBarView : UserControl
{
    private static readonly DependencyProperty InfoBarsProperty = Property<InfoBarView>.Depend<ObservableCollection<InfoBar>>(nameof(InfoBars));
    private readonly IInfoBarService infoBarService;

    /// <summary>
    /// 构造一个新的信息条视图
    /// </summary>
    public InfoBarView()
    {
        InitializeComponent();

        IServiceProvider serviceProvider = Ioc.Default;
        infoBarService = serviceProvider.GetRequiredService<IInfoBarService>();
        InfoBars = infoBarService.Collection;
        VisibilityButton.IsChecked = LocalSetting.Get(SettingKeys.IsInfoBarToggleChecked, true);
    }

    /// <summary>
    /// 信息条
    /// </summary>
    public ObservableCollection<InfoBar> InfoBars
    {
        get => (ObservableCollection<InfoBar>)GetValue(InfoBarsProperty);
        set => SetValue(InfoBarsProperty, value);
    }

    private void OnVisibilityButtonCheckedChanged(object sender, RoutedEventArgs e)
    {
        LocalSetting.Set(SettingKeys.IsInfoBarToggleChecked, ((ToggleButton)sender).IsChecked ?? false);
    }
}