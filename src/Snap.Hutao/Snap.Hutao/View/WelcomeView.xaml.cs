// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.ViewModel;

namespace Snap.Hutao.View;

/// <summary>
/// 欢迎视图
/// </summary>
[HighQuality]
internal sealed partial class WelcomeView : UserControl
{
    private readonly IServiceScope serviceScope;

    /// <summary>
    /// 构造一个新的欢迎视图
    /// </summary>
    public WelcomeView()
    {
        InitializeComponent();
        serviceScope = Ioc.Default.CreateScope();
        DataContext = serviceScope.ServiceProvider.GetRequiredService<WelcomeViewModel>();
    }

    private void OnUnloaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        DataContext = null;
        serviceScope.Dispose();
    }
}
