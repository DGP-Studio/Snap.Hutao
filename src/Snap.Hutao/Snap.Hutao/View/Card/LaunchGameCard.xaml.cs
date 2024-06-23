﻿// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.UI.Xaml;

namespace Snap.Hutao.View.Card;

/// <summary>
/// 启动游戏卡片
/// </summary>
internal sealed partial class LaunchGameCard : Button
{
    /// <summary>
    /// 构造一个新的启动游戏卡片
    /// </summary>
    public LaunchGameCard()
    {
        this.InitializeDataContext<ViewModel.Game.LaunchGameViewModelSlim>();
        InitializeComponent();
    }
}
