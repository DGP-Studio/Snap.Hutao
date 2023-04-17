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
    /// <summary>
    /// 已知的启动方案
    /// </summary>
    public static readonly ImmutableList<LaunchScheme> KnownSchemes = new List<LaunchScheme>()
    {
        // 官服
        new()
        {
            DisplayName = SH.ModelBindingLaunchGameLaunchSchemeChinese,
            LauncherId = "18",
            Key = "eYd89JmJ",
            Channel = ChannelType.Official,
            SubChannel = SubChannelType.Default,
            IsOversea = false,
        },

        // 渠道服
        new()
        {
            DisplayName = SH.ModelBindingLaunchGameLaunchSchemeBilibili,
            LauncherId = "17",
            Key = "KAtdSsoQ",
            Channel = ChannelType.Bili,
            SubChannel = SubChannelType.Default,
            IsOversea = false,
        },

        // 国际服
        new()
        {
            DisplayName = SH.ModelBindingLaunchGameLaunchSchemeOversea,
            LauncherId = "10",
            Key = "gcStgarh",
            Channel = ChannelType.Official,
            SubChannel = SubChannelType.Default,
            IsOversea = true,
        },
        new()
        {
            DisplayName = SH.ModelBindingLaunchGameLaunchSchemeOversea,
            LauncherId = "10",
            Key = "gcStgarh",
            Channel = ChannelType.Official,
            SubChannel = SubChannelType.Epic,
            IsOversea = true,
        },
        new()
        {
            DisplayName = SH.ModelBindingLaunchGameLaunchSchemeOversea,
            LauncherId = "10",
            Key = "gcStgarh",
            Channel = ChannelType.Official,
            SubChannel = SubChannelType.Google,
            IsOversea = true,
        },
    }.ToImmutableList();

    /// <summary>
    /// 名称
    /// </summary>
    public string DisplayName { get; private set; } = default!;

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
    public bool MultiChannelEqual(MultiChannel multiChannel)
    {
        return Channel == multiChannel.Channel && SubChannel == multiChannel.SubChannel;
    }
}