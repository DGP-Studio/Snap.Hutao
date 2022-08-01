// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Abstraction;

/// <summary>
/// 用户添加操作结果
/// </summary>
public enum UserAddResult
{
    /// <summary>
    /// 添加成功
    /// </summary>
    Added,

    /// <summary>
    /// 用户的Cookie成功更新
    /// </summary>
    Updated,

    /// <summary>
    /// 已经存在该用户
    /// </summary>
    AlreadyExists,
}