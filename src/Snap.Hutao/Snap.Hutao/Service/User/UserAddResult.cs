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
    Ok,

    /// <summary>
    /// 已经存在该用户
    /// </summary>
    AlreadyExists,

    /// <summary>
    /// 初始化用户失败
    /// </summary>
    InitializeFailed,
}