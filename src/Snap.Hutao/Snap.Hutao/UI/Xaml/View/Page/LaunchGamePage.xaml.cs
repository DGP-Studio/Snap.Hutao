// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.UI.Xaml.Control;
using Snap.Hutao.ViewModel.Game;

namespace Snap.Hutao.UI.Xaml.View.Page;

/// <summary>
/// 启动游戏页面
/// </summary>
[HighQuality]
internal sealed partial class LaunchGamePage : ScopedPage
{
    /// <summary>
    /// 构造一个新的启动游戏页面
    /// </summary>
    public LaunchGamePage()
    {
        InitializeWith<LaunchGameViewModel>();
        InitializeComponent();
    }
}
