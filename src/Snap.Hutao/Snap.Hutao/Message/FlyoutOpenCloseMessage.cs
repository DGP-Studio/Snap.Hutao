// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Message;

/// <summary>
/// Flyout开启关闭消息
/// </summary>
[HighQuality]
internal sealed class FlyoutOpenCloseMessage
{
    /// <summary>
    /// 构造一个新的Flyout开启关闭消息
    /// </summary>
    /// <param name="isOpen">是否为开启状态</param>
    public FlyoutOpenCloseMessage(bool isOpen)
    {
        IsOpen = isOpen;
    }

    /// <summary>
    /// 是否为开启状态
    /// </summary>
    public bool IsOpen { get; }
}