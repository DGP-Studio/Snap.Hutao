// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Windows.Win32.UI.WindowsAndMessaging;

namespace Snap.Hutao.Core.Windowing;

internal interface IMinMaxInfoHandler
{
    /// <summary>
    /// 处理最大最小信息
    /// </summary>
    /// <param name="info">信息</param>
    /// <param name="scalingFactor">缩放比</param>
    unsafe void HandleMinMaxInfo(ref MINMAXINFO info, double scalingFactor);
}