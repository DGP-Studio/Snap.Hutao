// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.UI.Xaml.Control;
using Snap.Hutao.ViewModel.Achievement;

namespace Snap.Hutao.UI.Xaml.View.Page;

/// <summary>
/// 成就页面
/// </summary>
[HighQuality]
internal sealed partial class AchievementPage : ScopedPage
{
    /// <summary>
    /// 构造一个新的成就页面
    /// </summary>
    public AchievementPage()
    {
        InitializeWith<AchievementViewModel>();
        InitializeComponent();
    }
}