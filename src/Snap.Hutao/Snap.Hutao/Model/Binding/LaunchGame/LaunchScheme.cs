// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Model.Binding.LaunchGame;

/// <summary>
/// 服务器方案
/// </summary>
/// <summary>
/// 启动方案
/// </summary>
public class LaunchScheme
{
    /// <summary>
    /// 构造一个新的启动方案
    /// </summary>
    /// <param name="name">名称</param>
    /// <param name="channel">通道</param>
    /// <param name="cps">通道描述字符串</param>
    /// <param name="subChannel">子通道</param>
    public LaunchScheme(string name, string channel, string subChannel)
    {
        Name = name;
        Channel = channel;
        SubChannel = subChannel;
    }

    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 通道
    /// </summary>
    public string Channel { get; set; }

    /// <summary>
    /// 子通道
    /// </summary>
    public string SubChannel { get; set; }
}