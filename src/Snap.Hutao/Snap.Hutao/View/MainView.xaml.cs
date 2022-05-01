// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Service.Abstraction;

namespace Snap.Hutao.View;

/// <summary>
/// 主视图
/// </summary>
public sealed partial class MainView : UserControl
{
    private readonly INavigationService navigationService;

    /// <summary>
    /// 构造一个新的主视图
    /// </summary>
    public MainView()
    {
        this.InitializeComponent();

        navigationService = Ioc.Default.GetRequiredService<INavigationService>();
        navigationService.Initialize(NavView, ContentFrame);
    }
}
