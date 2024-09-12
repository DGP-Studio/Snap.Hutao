﻿// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;

namespace Snap.Hutao.UI.Xaml.View.Card;

internal sealed partial class AchievementCard : Button
{
    public AchievementCard()
    {
        this.InitializeDataContext<ViewModel.Achievement.AchievementViewModelSlim>();
        InitializeComponent();
    }
}