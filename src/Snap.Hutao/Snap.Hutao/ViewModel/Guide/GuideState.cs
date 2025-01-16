// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.ViewModel.Guide;

/// <summary>
/// 指引状态
/// </summary>
internal enum GuideState : uint
{
    /// <summary>
    /// 选择语言
    /// </summary>
    Language,

    /// <summary>
    /// 查看文档与隐私政策
    /// </summary>
    Document,

    /// <summary>
    /// 查看环境配置
    /// </summary>
    Environment,

    /// <summary>
    /// 查看数据文件夹
    /// </summary>
    DataFolder,

    /// <summary>
    /// 查看常用设置
    /// </summary>
    CommonSetting,

    /// <summary>
    /// 查看图像资源设置
    /// </summary>
    StaticResourceSetting,

    /// <summary>
    /// 开始下载图像资源
    /// </summary>
    StaticResourceBegin,

    /// <summary>
    /// 完成
    /// </summary>
    Completed,
}