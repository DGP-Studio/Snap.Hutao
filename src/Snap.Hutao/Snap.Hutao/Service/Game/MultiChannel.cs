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
    /// 配置文件路径 当不为 null 时则存在文件读写问题
    /// </summary>
    public string? ConfigFilePath;

    /// <summary>
    /// 构造一个新的多通道
    /// </summary>
    /// <param name="channel">通道</param>
    /// <param name="subChannel">子通道</param>
    /// <param name="configFilePath">配置文件路径</param>
    public MultiChannel(string? channel, string? subChannel, string? configFilePath = null)
    {
        Channel = channel ?? string.Empty;
        SubChannel = subChannel ?? string.Empty;
        ConfigFilePath = configFilePath;
    }
}