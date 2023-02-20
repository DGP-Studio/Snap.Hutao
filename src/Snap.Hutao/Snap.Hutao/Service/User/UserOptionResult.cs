// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.User;

/// <summary>
/// 用户添加操作结果
/// </summary>
[HighQuality]
internal enum UserOptionResult
{
    /// <summary>
    /// 添加成功
    /// </summary>
    Added,

    /// <summary>
    /// Cookie不完整
    /// </summary>
    Incomplete,

    /// <summary>
    /// Cookie信息已经失效
    /// </summary>
    Invalid,

    /// <summary>
    /// 用户的Cookie成功更新
    /// </summary>
    Updated,
}