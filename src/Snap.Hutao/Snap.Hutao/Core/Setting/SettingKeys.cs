// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.Setting;

/// <summary>
/// 设置键
/// </summary>
[HighQuality]
[SuppressMessage("", "SA1124")]
internal static class SettingKeys
{
    /// <summary>
    /// 窗体矩形
    /// </summary>
    public const string WindowRect = "WindowRect";

    /// <summary>
    /// 导航侧栏是否展开
    /// </summary>
    public const string IsNavPaneOpen = "IsNavPaneOpen";

    /// <summary>
    /// 启动次数
    /// </summary>
    public const string LaunchTimes = "LaunchTimes";

    /// <summary>
    /// 数据文件夹
    /// </summary>
    public const string DataFolderPath = "DataFolderPath";

    /// <summary>
    /// 通行证用户名（邮箱）
    /// </summary>
    public const string PassportUserName = "PassportUserName";

    /// <summary>
    /// 通行证密码
    /// </summary>
    public const string PassportPassword = "PassportPassword";

    /// <summary>
    /// 消息是否显示
    /// </summary>
    public const string IsInfoBarToggleChecked = "IsInfoBarToggleChecked";

    #region StaticResource

    /// <summary>
    /// 静态资源合约
    /// 新增合约时 请注意
    /// <see cref="StaticResource.FulfillAllContracts"/>
    /// 与 <see cref="StaticResource.IsAnyUnfulfilledContractPresent"/>
    /// </summary>
    public const string StaticResourceV1Contract = "StaticResourceV1Contract";

    /// <summary>
    /// 静态资源合约V2 成就图标与物品图标
    /// </summary>
    public const string StaticResourceV2Contract = "StaticResourceV2Contract";

    /// <summary>
    /// 静态资源合约V3 刷新 Skill Talent
    /// </summary>
    public const string StaticResourceV3Contract = "StaticResourceV3Contract";

    /// <summary>
    /// 静态资源合约V4 刷新 AvatarIcon
    /// </summary>
    public const string StaticResourceV4Contract = "StaticResourceV4Contract";

    /// <summary>
    /// 静态资源合约V5 刷新 AvatarIcon
    /// </summary>
    public const string StaticResourceV5Contract = "StaticResourceV5Contract";

    /// <summary>
    /// 是否切换到 星穹铁道 服务器
    /// </summary>
    public const string IsSwitchToStarRailTool = "IsSwitchToStarRailTool";

    /// <summary>
    /// 是否加入自启动计划
    /// </summary>
    public const string IsIncludeSelfStart = "IsIncludeSelfStart";
    #endregion
}