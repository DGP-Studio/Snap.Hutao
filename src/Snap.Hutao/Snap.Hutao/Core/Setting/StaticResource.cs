// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.Setting;

/// <summary>
/// 静态资源
/// </summary>
[HighQuality]
internal static class StaticResource
{
    /// <summary>
    /// 完成所有合约
    /// </summary>
    public static void FulfillAllContracts()
    {
        SetContractsState(true);
    }

    /// <summary>
    /// 完成所有合约
    /// </summary>
    public static void UnfulfillAllContracts()
    {
        SetContractsState(false);
    }

    /// <summary>
    /// 提供的合约是否未完成
    /// </summary>
    /// <param name="contractKey">合约的键</param>
    /// <returns>合约是否未完成</returns>
    public static bool IsContractUnfulfilled(string contractKey)
    {
        return !LocalSetting.Get(contractKey, false);
    }

    /// <summary>
    /// 是否有任何静态资源合约尚未完成
    /// </summary>
    /// <returns>静态资源合约尚未完成</returns>
    public static bool IsAnyUnfulfilledContractPresent()
    {
        return !LocalSetting.Get(SettingKeys.StaticResourceV1Contract, false)
            || (!LocalSetting.Get(SettingKeys.StaticResourceV2Contract, false))
            || (!LocalSetting.Get(SettingKeys.StaticResourceV3Contract, false))
            || (!LocalSetting.Get(SettingKeys.StaticResourceV4Contract, false));
    }

    /// <summary>
    /// 是否切换到 星穹铁道 服务器
    /// </summary>
    /// <returns>是否切换</returns>
    public static bool IsSwitchToStarRailTool()
    {
        return LocalSetting.Get(SettingKeys.IsSwitchToStarRailTool, false);
    }

    /// <summary>
    /// 星穹铁道 服务器 和 原神 服务器之间的切换
    /// </summary>
    public static void SwitchBetweenStarRailOrGenshin()
    {
        LocalSetting.Set(SettingKeys.IsSwitchToStarRailTool, !IsSwitchToStarRailTool());
    }

    /// <summary>
    /// 判断是否需要自启动或者设置状态
    /// </summary>
    /// <param name="set">是否设置</param>
    /// <param name="state">状态</param>
    /// <returns>是否需要自启动</returns>
    public static bool IsIncludeSelfStartOrSetState(bool set = false, bool state = false)
    {
        if (set)
        {
            LocalSetting.Set(SettingKeys.IsIncludeSelfStart, state);
        }

        return LocalSetting.Get(SettingKeys.IsIncludeSelfStart, false);
    }

    private static void SetContractsState(bool state)
    {
        LocalSetting.Set(SettingKeys.StaticResourceV1Contract, state);
        LocalSetting.Set(SettingKeys.StaticResourceV2Contract, state);
        LocalSetting.Set(SettingKeys.StaticResourceV3Contract, state);
        LocalSetting.Set(SettingKeys.StaticResourceV4Contract, state);
        LocalSetting.Set(SettingKeys.StaticResourceV5Contract, state);
    }
}