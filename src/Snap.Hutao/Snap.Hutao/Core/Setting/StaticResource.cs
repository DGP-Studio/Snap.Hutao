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
    /// 取消完成所有合约
    /// </summary>
    public static void FailAllContracts()
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

    private static void SetContractsState(bool state)
    {
        LocalSetting.Set(SettingKeys.StaticResourceV1Contract, state);
        LocalSetting.Set(SettingKeys.StaticResourceV2Contract, state);
        LocalSetting.Set(SettingKeys.StaticResourceV3Contract, state);
        LocalSetting.Set(SettingKeys.StaticResourceV4Contract, state);
        LocalSetting.Set(SettingKeys.StaticResourceV5Contract, state);
    }
}