// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.LifeCycle;
using Windows.UI.StartScreen;

namespace Snap.Hutao.Core;

/// <summary>
/// 跳转列表帮助类
/// </summary>
public static class JumpListHelper
{
    /// <summary>
    /// 异步配置跳转列表
    /// </summary>
    /// <returns>任务</returns>
    public static async Task ConfigureAsync()
    {
        if (JumpList.IsSupported())
        {
            JumpList list = await JumpList.LoadCurrentAsync();

            list.Items.Clear();

            JumpListItem launchGameItem = JumpListItem.CreateWithArguments(Activation.LaunchGame, SH.CoreJumpListHelperLaunchGameItemDisplayName);
            launchGameItem.GroupName = SH.CoreJumpListHelperLaunchGameItemGroupName;
            launchGameItem.Logo = new("ms-appx:///Resource/Icon/UI_GuideIcon_PlayMethod.png");

            list.Items.Add(launchGameItem);

            await list.SaveAsync();
        }
    }
}