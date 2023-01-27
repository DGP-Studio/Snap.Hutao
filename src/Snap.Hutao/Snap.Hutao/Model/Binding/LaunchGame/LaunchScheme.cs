// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Immutable;

namespace Snap.Hutao.Model.Binding.LaunchGame;

/// <summary>
/// 启动方案
/// </summary>
public class LaunchScheme
{
    /// <summary>
    /// 已知的启动方案
    /// </summary>
    public static readonly ImmutableList<LaunchScheme> KnownSchemes = new List<LaunchScheme>()
    {
        new LaunchScheme("官方服 | 天空岛", "eYd89JmJ", "18", "1", "1"),
        new LaunchScheme("渠道服 | 世界树", "KAtdSsoQ", "17", "14", "0"),
        new LaunchScheme("国际服 | 部分支持", "gcStgarh", "10", "1", "0"),
    }.ToImmutableList();

    /// <summary>
    /// 构造一个新的启动方案
    /// </summary>
    /// <param name="displayName">名称</param>
    /// <param name="channel">通道</param>
    /// <param name="subChannel">子通道</param>
    /// <param name="launcherId">启动器Id</param>
    private LaunchScheme(string displayName, string key, string launcherId, string channel, string subChannel)
    {
        DisplayName = displayName;
        Channel = channel;
        SubChannel = subChannel;
        LauncherId = launcherId;
        Key = key;
    }

    /// <summary>
    /// 名称
    /// </summary>
    public string DisplayName { get; set; }

    /// <summary>
    /// 通道
    /// </summary>
    public string Channel { get; set; }

    /// <summary>
    /// 子通道
    /// </summary>
    public string SubChannel { get; set; }

    /// <summary>
    /// 启动器Id
    /// </summary>
    public string LauncherId { get; set; }

    /// <summary>
    /// API Key
    /// </summary>
    public string Key { get; set; }

    /// <summary>
    /// 是否为海外
    /// </summary>
    public bool IsOversea { get => LauncherId == "10"; }
}