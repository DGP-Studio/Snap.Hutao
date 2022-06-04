// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.SpiralAbyss;

namespace Snap.Hutao.Web.Hutao.Model.Post;

/// <summary>
/// 带有层信息的间
/// </summary>
internal class IndexedLevel
{
    /// <summary>
    /// 构造一个新的带有层信息的间
    /// </summary>
    /// <param name="floorIndex">层号</param>
    /// <param name="level">间信息</param>
    public IndexedLevel(int floorIndex, Level level)
    {
        FloorIndex = floorIndex;
        Level = level;
    }

    /// <summary>
    /// 层号
    /// </summary>
    public int FloorIndex { get; }

    /// <summary>
    /// 层信息
    /// </summary>
    public Level Level { get; }
}
