// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;

namespace Snap.Hutao.Service.Game;

/// <summary>
/// 多通道
/// </summary>
[HighQuality]
internal readonly struct MultiChannel
{
    /// <summary>
    /// 通道
    /// </summary>
    public readonly ChannelType Channel;

    /// <summary>
    /// 子通道
    /// </summary>
    public readonly SubChannelType SubChannel;

    /// <summary>
    /// 配置文件路径 当不为 null 时则存在文件读写问题
    /// </summary>
    public readonly string? ConfigFilePath;

    /// <summary>
    /// 构造一个新的多通道
    /// </summary>
    /// <param name="channel">通道</param>
    /// <param name="subChannel">子通道</param>
    /// <param name="configFilePath">配置文件路径</param>
    public MultiChannel(string? channel, string? subChannel, string? configFilePath = null)
    {
        Channel = string.IsNullOrEmpty(channel) ? ChannelType.Default : Enum.Parse<ChannelType>(channel);
        SubChannel = string.IsNullOrEmpty(subChannel) ? SubChannelType.Default : Enum.Parse<SubChannelType>(subChannel);

        ConfigFilePath = configFilePath;
    }
}