// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model;
using Snap.Hutao.Web.Hoyolab.Hk4e.Event.GachaInfo;

namespace Snap.Hutao.Service.GachaLog;

/// <summary>
/// 祈愿记录获取状态
/// </summary>
internal sealed class GachaLogFetchStatus
{
    /// <summary>
    /// 构造一个新的祈愿记录获取状态
    /// </summary>
    /// <param name="configType">卡池类型</param>
    public GachaLogFetchStatus(GachaConfigType configType)
    {
        ConfigType = configType;
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
    public List<Item> Items { get; set; } = new(20);

    public string Header
    {
        get
        {
            return AuthKeyTimeout
                ? SH.ViewDialogGachaLogRefreshProgressAuthkeyTimeout
                : SH.ViewDialogGachaLogRefreshProgressDescription.Format(ConfigType.GetLocalizedDescription());
        }
    }
}