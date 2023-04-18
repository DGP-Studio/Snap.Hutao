// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.ViewModel.Achievement;

/// <summary>
/// 简化的成就视图模型
/// </summary>
internal sealed class AchievementViewModelSlim : Abstraction.ViewModelSlim<View.Page.AchievementPage>
{
    /// <summary>
    /// 构造一个新的简化的成就视图模型
    /// </summary>
    /// <param name="serviceProvider">服务提供器</param>
    public AchievementViewModelSlim(IServiceProvider serviceProvider)
        : base(serviceProvider)
    {
    }
}