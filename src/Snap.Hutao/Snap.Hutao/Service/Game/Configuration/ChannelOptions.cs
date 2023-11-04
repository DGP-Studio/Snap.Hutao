// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;

namespace Snap.Hutao.Service.Game.Configuration;

/// <summary>
/// 多通道
/// </summary>
[HighQuality]
internal readonly struct ChannelOptions
{
    public const string ChannelName = "channel";
    public const string SubChannelName = "sub_channel";

    /// <summary>
    /// 通道
    /// </summary>
    public readonly ChannelType Channel;

    /// <summary>
    /// 子通道
    /// </summary>
    public readonly SubChannelType SubChannel;

    /// <summary>
    /// 是否为国际服
    /// </summary>
    public readonly bool IsOversea;

    /// <summary>
    /// 配置文件路径 当不为 null 时则存在文件读写问题
    /// </summary>
    public readonly string? ConfigFilePath;

    /// <summary>
    /// 构造一个新的多通道
    /// </summary>
    /// <param name="channel">通道</param>
    /// <param name="subChannel">子通道</param>
    /// <param name="isOversea">是否为国际服</param>
    /// <param name="configFilePath">配置文件路径</param>
    public ChannelOptions(string? channel, string? subChannel, bool isOversea, string? configFilePath = null)
    {
        _ = Enum.TryParse(channel, out Channel);
        _ = Enum.TryParse(subChannel, out SubChannel);
        IsOversea = isOversea;
        ConfigFilePath = configFilePath;
    }

    public ChannelOptions(ChannelType channel, SubChannelType subChannel, bool isOversea)
    {
        Channel = channel;
        SubChannel = subChannel;
        IsOversea = isOversea;
    }

    /// <summary>
    /// 配置文件未找到
    /// </summary>
    /// <param name="isOversea">是否为国际服</param>
    /// <param name="configFilePath">配置文件期望路径</param>
    /// <returns>选项</returns>
    public static ChannelOptions FileNotFound(bool isOversea, string configFilePath)
    {
        return new(null, null, isOversea, configFilePath);
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"[ChannelType:{Channel}] [SubChannel:{SubChannel}] [IsOversea: {IsOversea}]";
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Channel, SubChannel, IsOversea);
    }
}