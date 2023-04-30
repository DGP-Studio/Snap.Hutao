// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;
using System.Collections.Immutable;

namespace Snap.Hutao.Service.Game;

/// <summary>
/// 启动方案
/// </summary>
[HighQuality]
internal sealed class LaunchScheme
{
    // TODO: fix detection

    /// <summary>
    /// 已知的启动方案
    /// </summary>
    public static readonly ImmutableList<LaunchScheme> KnownSchemes = new List<LaunchScheme>()
    {
        // 官服
        new()
        {
            LauncherId = "18",
            Key = "eYd89JmJ",
            Channel = ChannelType.Official,
            SubChannel = SubChannelType.Official,
            IsOversea = false,
        },
        new()
        {
            LauncherId = "18",
            Key = "eYd89JmJ",
            Channel = ChannelType.Official,
            SubChannel = SubChannelType.NoTapTap,
            IsOversea = false,
        },

        // 渠道服
        new()
        {
            LauncherId = "17",
            Key = "KAtdSsoQ",
            Channel = ChannelType.Bili,
            SubChannel = SubChannelType.Default,
            IsOversea = false,
        },

        // 国际服
        new()
        {
            LauncherId = "10",
            Key = "gcStgarh",
            Channel = ChannelType.Official,
            SubChannel = SubChannelType.Default,
            IsOversea = true,
        },
        new()
        {
            LauncherId = "10",
            Key = "gcStgarh",
            Channel = ChannelType.Official,
            SubChannel = SubChannelType.Epic,
            IsOversea = true,
        },
        new()
        {
            LauncherId = "10",
            Key = "gcStgarh",
            Channel = ChannelType.Official,
            SubChannel = SubChannelType.Google,
            IsOversea = true,
        },
    }.ToImmutableList();

    /// <summary>
    /// 显示名称
    /// </summary>
    public string DisplayName
    {
        get
        {
            return (Channel, IsOversea) switch
            {
                (ChannelType.Official, false) => SH.ModelBindingLaunchGameLaunchSchemeChinese,
                (ChannelType.Bili, false) => SH.ModelBindingLaunchGameLaunchSchemeBilibili,
                (_, true) => $"{SH.ModelBindingLaunchGameLaunchSchemeOversea} | {SubChannel}",
                _ => throw Must.NeverHappen(),
            };
        }
    }

    /// <summary>
    /// 通道
    /// </summary>
    public ChannelType Channel { get; private set; }

    /// <summary>
    /// 子通道
    /// </summary>
    public SubChannelType SubChannel { get; private set; }

    /// <summary>
    /// 启动器 Id
    /// </summary>
    public string LauncherId { get; private set; } = default!;

    /// <summary>
    /// API Key
    /// </summary>
    public string Key { get; private set; } = default!;

    /// <summary>
    /// 是否为海外
    /// </summary>
    public bool IsOversea { get; private set; }

    /// <summary>
    /// 多通道相等
    /// </summary>
    /// <param name="multiChannel">多通道</param>
    /// <returns>是否相等</returns>
    public bool MultiChannelEqual(in MultiChannel multiChannel)
    {
        return Channel == multiChannel.Channel && SubChannel == multiChannel.SubChannel;
    }
}