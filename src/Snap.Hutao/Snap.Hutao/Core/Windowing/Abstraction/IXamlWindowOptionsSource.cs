// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.Windowing.Abstraction;

/// <summary>
/// 为扩展窗体提供必要的选项
/// </summary>
internal interface IXamlWindowOptionsSource
{
    /// <summary>
    /// 窗体选项
    /// </summary>
    XamlWindowOptions WindowOptions { get; }
}