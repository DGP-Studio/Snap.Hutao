// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Control.Cancellable;
using Snap.Hutao.ViewModel;

namespace Snap.Hutao.View.Page;

/// <summary>
/// 成就页面
/// </summary>
public sealed partial class AchievementPage : CancellablePage
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
