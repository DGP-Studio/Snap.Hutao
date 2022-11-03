// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Windows.Graphics;
using Windows.Win32.UI.WindowsAndMessaging;

namespace Snap.Hutao.Core.Windowing;

/// <summary>
/// 为扩展窗体提供必要的选项
/// </summary>
/// <typeparam name="TWindow">窗体类型</typeparam>
internal interface IExtendedWindowSource
{
    /// <summary>
    /// 提供的标题栏
    /// </summary>
    FrameworkElement TitleBar { get; }

    /// <summary>
    /// 是否持久化尺寸
    /// </summary>
    bool PersistSize { get; }

    /// <summary>
    /// 初始大小
    /// </summary>
    SizeInt32 InitSize { get; }

    /// <summary>
    /// 处理最大最小信息
    /// </summary>
    /// <param name="pInfo">信息指针</param>
    /// <param name="scalingFactor">缩放比</param>
    unsafe void ProcessMinMaxInfo(MINMAXINFO* pInfo, double scalingFactor);
}