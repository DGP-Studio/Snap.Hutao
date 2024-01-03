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

    public ChannelOptions(ChannelType channel, SubChannelType subChannel, bool isOversea)
    {
        Channel = channel;
        SubChannel = subChannel;
        IsOversea = isOversea;
    }

    public ChannelOptions(string? channel, string? subChannel, bool isOversea)
    {
        _ = Enum.TryParse(channel, out Channel);
        _ = Enum.TryParse(subChannel, out SubChannel);
        IsOversea = isOversea;
    }

    private ChannelOptions(bool isOversea, string? configFilePath)
    {
        IsOversea = isOversea;
        ConfigFilePath = configFilePath;
    }

    public static ChannelOptions FileNotFound(bool isOversea, string configFilePath)
    {
        return new(isOversea, configFilePath);
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $$"""
            { ChannelType: {{Channel}}, SubChannel: {{SubChannel}}, IsOversea: {{IsOversea}}}
            """;
    }

    // DO NOT DELETE, used in HashSet
    public override int GetHashCode()
    {
        return HashCode.Combine(Channel, SubChannel, IsOversea);
    }
}