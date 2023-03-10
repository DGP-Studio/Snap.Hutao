// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Game;

namespace Snap.Hutao.ViewModel.Game;

/// <summary>
/// 截图视图模型
/// </summary>
[Injection(InjectAs.Scoped)]
internal sealed class ScreenshotViewModel : Abstraction.ViewModel
{
    /// <summary>
    /// 构造一个新的截图视图模型
    /// </summary>
    /// <param name="gameService">游戏服务</param>
    public ScreenshotViewModel(IGameService gameService)
    {

    }

    /// <inheritdoc/>
    protected override async Task OpenUIAsync()
    {

    }
}