// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Binding.Gacha.Abstraction;
using Snap.Hutao.Web.Hoyolab.Hk4e.Event.GachaInfo;

namespace Snap.Hutao.Service.GachaLog;

/// <summary>
/// 获取状态
/// </summary>
public class FetchState
{
    /// <summary>
    /// 初始化一个新的获取状态
    /// </summary>
    public FetchState()
    {
        Items = new(20);
    }

    /// <summary>
    /// 验证密钥是否过期
    /// </summary>
    public bool AuthKeyTimeout { get; set; }

    /// <summary>
    /// 卡池类型
    /// </summary>
    public GachaConfigType ConfigType { get; set; }

    /// <summary>
    /// 当前获取的物品
    /// </summary>
    public List<ItemBase> Items { get; set; }
}