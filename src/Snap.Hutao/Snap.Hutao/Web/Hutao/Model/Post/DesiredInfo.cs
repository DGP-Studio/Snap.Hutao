// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.SpiralAbyss;
using System.Collections.Generic;

namespace Snap.Hutao.Web.Hutao.Model.Post;

/// <summary>
/// 封装期望楼层与角色列表
/// </summary>
public class DesiredInfo
{
    /// <summary>
    /// 构造一个新的封装类
    /// </summary>
    /// <param name="floor">楼层</param>
    /// <param name="desiredAvatars">期望角色，按期望顺序排序</param>
    public DesiredInfo(int floor, IEnumerable<string> desiredAvatars)
    {
        Floor = floor;
        DesiredAvatars = desiredAvatars;
    }

    /// <summary>
    /// 层
    /// </summary>
    public int Floor { get; set; }

    /// <summary>
    /// 期望角色
    /// </summary>
    public IEnumerable<string> DesiredAvatars { get; set; }
}