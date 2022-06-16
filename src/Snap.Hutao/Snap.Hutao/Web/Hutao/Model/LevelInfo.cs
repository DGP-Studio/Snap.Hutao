// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hutao.Model;

/// <summary>
/// 带有层的间编号
/// </summary>
public class LevelInfo
{
    /// <summary>
    /// 层
    /// </summary>
    public int Floor { get; set; }

    /// <summary>
    /// 上下半 0,1
    /// </summary>
    public int Index { get; set; }
}
