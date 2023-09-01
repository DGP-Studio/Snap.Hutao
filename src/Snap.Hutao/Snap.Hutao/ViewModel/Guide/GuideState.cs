// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.ViewModel.Guide;

/// <summary>
/// 指引状态
/// </summary>
internal enum GuideState : uint
{
    /// <summary>
    /// 正在选择语言
    /// </summary>
    Language,

    /// <summary>
    /// 正在查看文档与隐私政策
    /// </summary>
    Document,

    /// <summary>
    /// 正在查看环境配置
    /// </summary>
    Environment,

    /// <summary>
    /// 开始下载资源
    /// </summary>
    StaticResourceBegin,

    /// <summary>
    /// 完成
    /// </summary>
    Completed,
}