// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Windows.Win32.UI.WindowsAndMessaging;

namespace Snap.Hutao.Core.Windowing;

/// <summary>
/// 为扩展窗体提供必要的选项
/// </summary>
/// <typeparam name="TWindow">窗体类型</typeparam>
internal interface IWindowOptionsSource
{
    /// <summary>
    /// 窗体选项
    /// </summary>
    WindowOptions WindowOptions { get; }

    /// <summary>
    /// 处理最大最小信息
    /// </summary>
    /// <param name="pInfo">信息指针</param>
    /// <param name="scalingFactor">缩放比</param>
    unsafe void ProcessMinMaxInfo(MINMAXINFO* pInfo, double scalingFactor);
}