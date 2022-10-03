// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.AvatarInfo;

/// <summary>
/// 刷新结果
/// </summary>
public enum RefreshResult
{
    /// <summary>
    /// 正常
    /// </summary>
    Ok,

    /// <summary>
    /// API 不可用
    /// </summary>
    APIUnavailable,

    /// <summary>
    /// 角色橱窗未对外开放
    /// </summary>
    ShowcaseNotOpen,
}