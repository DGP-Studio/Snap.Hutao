// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.GachaLog;

/// <summary>
/// 刷新选项
/// </summary>
[HighQuality]
internal enum RefreshOption
{
    /// <summary>
    /// 无模式刷新
    /// 用于返回新的统计数据
    /// 或者切换存档后的刷新
    /// </summary>
    None,

    /// <summary>
    /// 通过浏览器缓存刷新
    /// </summary>
    WebCache,

    /// <summary>
    /// 通过Stoken刷新
    /// </summary>
    Stoken,

    /// <summary>
    /// 手动输入Url刷新
    /// </summary>
    ManualInput,
}
