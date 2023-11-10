// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Service.Game.Configuration;

namespace Snap.Hutao.Service.Game.Scheme;

/// <summary>
/// 启动方案
/// </summary>
[HighQuality]
internal class LaunchScheme : IEquatable<ChannelOptions>
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
    public ChannelType Channel { get; private protected set; }

    /// <summary>
    /// 子通道
    /// </summary>
    public SubChannelType SubChannel { get; private protected set; }

    /// <summary>
    /// 启动器 Id
    /// </summary>
    public int LauncherId { get; private protected set; }

    /// <summary>
    /// API Key
    /// </summary>
    public string Key { get; private protected set; } = default!;

    /// <summary>
    /// 是否为海外
    /// </summary>
    public bool IsOversea { get; private protected set; }

    public bool IsNotCompatOnly { get; private protected set; } = true;

    public static bool ExecutableIsOversea(string gameFileName)
    {
        return gameFileName switch
        {
            GameConstants.GenshinImpactFileName => true,
            GameConstants.YuanShenFileName => false,
            _ => throw Requires.Fail("无效的游戏可执行文件名称：{0}", gameFileName),
        };
    }

    [SuppressMessage("", "SH002")]
    public bool Equals(ChannelOptions other)
    {
        return Channel == other.Channel && SubChannel == other.SubChannel;
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