// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hutao.Log;

/// <summary>
/// 胡桃日志
/// </summary>
public class HutaoLog
{
    /// <summary>
    /// 设备Id
    /// </summary>
    public string Id { get; set; } = default!;

    /// <summary>
    /// 崩溃时间
    /// </summary>
    public long Time { get; set; }

    /// <summary>
    /// 错误信息
    /// </summary>
    public string Info { get; set; } = default!;
}
