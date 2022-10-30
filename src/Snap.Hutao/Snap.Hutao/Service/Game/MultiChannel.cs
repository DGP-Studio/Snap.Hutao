// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Game;

/// <summary>
/// 多通道
/// </summary>
public struct MultiChannel
{
    /// <summary>
    /// 通道
    /// </summary>
    public string Channel;

    /// <summary>
    /// 子通道
    /// </summary>
    public string SubChannel;

    /// <summary>
    /// 构造一个新的多通道
    /// </summary>
    /// <param name="channel">通道</param>
    /// <param name="subChannel">子通道</param>
    public MultiChannel(string? channel, string? subChannel)
    {
        Channel = channel ?? string.Empty;
        SubChannel = subChannel ?? string.Empty;
    }
}