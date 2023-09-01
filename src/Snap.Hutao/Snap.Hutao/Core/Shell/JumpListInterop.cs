// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.LifeCycle;
using Windows.UI.StartScreen;

namespace Snap.Hutao.Core.Shell;

/// <summary>
/// 跳转列表交互
/// </summary>
[HighQuality]
[Injection(InjectAs.Transient, typeof(IJumpListInterop))]
internal sealed class JumpListInterop : IJumpListInterop
{
    /// <summary>
    /// 异步配置跳转列表
    /// </summary>
    /// <returns>任务</returns>
    public async ValueTask ConfigureAsync()
    {
        if (JumpList.IsSupported())
        {
            JumpList list = await JumpList.LoadCurrentAsync();

            list.Items.Clear();

            JumpListItem launchGameItem = JumpListItem.CreateWithArguments(Activation.LaunchGame, SH.CoreJumpListHelperLaunchGameItemDisplayName);
            launchGameItem.Logo = "ms-appx:///Resource/Navigation/LaunchGame.png".ToUri();

            list.Items.Add(launchGameItem);

            await list.SaveAsync();
        }
    }
}