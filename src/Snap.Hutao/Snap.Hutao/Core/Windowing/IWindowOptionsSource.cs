// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.Windowing;

/// <summary>
/// 为扩展窗体提供必要的选项
/// </summary>
internal interface IWindowOptionsSource
{
    /// <summary>
    /// 窗体选项
    /// </summary>
    WindowOptions WindowOptions { get; }
}