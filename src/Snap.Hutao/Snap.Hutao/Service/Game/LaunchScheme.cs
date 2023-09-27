// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;

namespace Snap.Hutao.Service.Game;

/// <summary>
/// 启动方案
/// </summary>
[HighQuality]
internal sealed partial class LaunchScheme
{
    /// <summary>
    /// 显示名称
    /// </summary>
    public string DisplayName
    {
        get
        {
            string name = (Channel, IsOversea) switch
            {
                (ChannelType.Bili, false) => SH.ModelBindingLaunchGameLaunchSchemeBilibili,
                (_, false) => SH.ModelBindingLaunchGameLaunchSchemeChinese,
                (_, true) => SH.ModelBindingLaunchGameLaunchSchemeOversea,
            };

            return $"{name} | {Channel} | {SubChannel}";
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
    public int LauncherId { get; private set; }

    /// <summary>
    /// API Key
    /// </summary>
    public string Key { get; private set; } = default!;

    /// <summary>
    /// 是否为海外
    /// </summary>
    public bool IsOversea { get; private set; }

    public bool IsNotCompatOnly { get; private set; } = true;

    /// <summary>
    /// 多通道相等
    /// </summary>
    /// <param name="multiChannel">多通道</param>
    /// <returns>是否相等</returns>
    public bool MultiChannelEqual(in ChannelOptions multiChannel)
    {
        return Channel == multiChannel.Channel && SubChannel == multiChannel.SubChannel;
    }

    public bool ExecutableMatches(string gameFileName)
    {
        return (IsOversea, gameFileName) switch
        {
            (true, GameConstants.GenshinImpactFileName) => true,
            (false, GameConstants.YuanShenFileName) => true,
            _ => false,
        };
    }
}