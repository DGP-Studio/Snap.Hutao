// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Control;
using Snap.Hutao.ViewModel;

namespace Snap.Hutao.View.Page;

/// <summary>
/// 启动游戏页面
/// </summary>
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
