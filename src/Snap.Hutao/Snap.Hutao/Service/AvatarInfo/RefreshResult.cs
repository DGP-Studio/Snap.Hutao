// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.AvatarInfo;

/// <summary>
/// 刷新结果
/// </summary>
[HighQuality]
internal enum RefreshResult
{
    /// <summary>
    /// 正常
    /// </summary>
    Ok,

    /// <summary>
    /// 元数据加载失败
    /// </summary>
    MetadataNotInitialized,

    /// <summary>
    /// API 不可用
    /// </summary>
    APIUnavailable,

    /// <summary>
    /// 状态码异常
    /// </summary>
    StatusCodeNotSucceed,

    /// <summary>
    /// 角色橱窗未对外开放
    /// </summary>
    ShowcaseNotOpen,
}