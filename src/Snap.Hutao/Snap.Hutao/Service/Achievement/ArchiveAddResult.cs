// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Achievement;

/// <summary>
/// 存档添加结果
/// </summary>
[HighQuality]
internal enum ArchiveAddResult
{
    /// <summary>
    /// 添加成功
    /// </summary>
    Added,

    /// <summary>
    /// 名称无效
    /// </summary>
    InvalidName,

    /// <summary>
    /// 已经存在该存档
    /// </summary>
    AlreadyExists,
}