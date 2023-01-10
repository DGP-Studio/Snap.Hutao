// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.ViewModel;

namespace Snap.Hutao.View;

/// <summary>
/// 欢迎视图
/// </summary>
public sealed partial class WelcomeView : UserControl
{
    /// <summary>
    /// 构造一个新的欢迎视图
    /// </summary>
    public WelcomeView()
    {
        InitializeComponent();
        DataContext = Ioc.Default.GetRequiredService<WelcomeViewModel>();
    }
}
