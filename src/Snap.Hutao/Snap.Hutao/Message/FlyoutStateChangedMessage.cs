// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Message;

/// <summary>
/// Flyout开启关闭消息
/// </summary>
[HighQuality]
internal sealed class FlyoutStateChangedMessage
{
    /// <summary>
    /// 构造一个新的Flyout开启关闭消息
    /// </summary>
    /// <param name="isOpen">是否为开启状态</param>
    public FlyoutStateChangedMessage(bool isOpen)
    {
        IsOpen = isOpen;
    }

    public static FlyoutStateChangedMessage Open { get; } = new(true);

    public static FlyoutStateChangedMessage Close { get; } = new(false);


    /// <summary>
    /// 是否为开启状态
    /// </summary>
    public bool IsOpen { get; }
}