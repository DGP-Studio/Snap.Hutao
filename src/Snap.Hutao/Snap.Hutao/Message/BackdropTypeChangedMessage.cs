// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Windowing;

namespace Snap.Hutao.Message;

/// <summary>
/// 背景类型改变消息
/// </summary>
internal class BackdropTypeChangedMessage
{
    /// <summary>
    /// 构造一个新的背景类型改变消息
    /// </summary>
    /// <param name="backdropType">背景类型</param>
    public BackdropTypeChangedMessage(BackdropType backdropType)
    {
        BackdropType = backdropType;
    }

    /// <summary>
    /// 背景类型
    /// </summary>
    public BackdropType BackdropType { get; set; }
}