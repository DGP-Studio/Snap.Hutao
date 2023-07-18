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
    /// 静态资源合约
    /// </summary>
    public const string V1Contract = "StaticResourceV1Contract";

    /// <summary>
    /// 静态资源合约V2 成就图标与物品图标
    /// </summary>
    public const string V2Contract = "StaticResourceV2Contract";

    /// <summary>
    /// 静态资源合约V3 刷新 Skill Talent
    /// </summary>
    public const string V3Contract = "StaticResourceV3Contract";

    /// <summary>
    /// 静态资源合约V4 刷新 AvatarIcon
    /// </summary>
    public const string V4Contract = "StaticResourceV4Contract";

    /// <summary>
    /// 静态资源合约V5 刷新 AvatarIcon
    /// </summary>
    public const string V5Contract = "StaticResourceV5Contract";

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
        return !LocalSetting.Get(V1Contract, false)
            || (!LocalSetting.Get(V2Contract, false))
            || (!LocalSetting.Get(V3Contract, false))
            || (!LocalSetting.Get(V4Contract, false))
            || (!LocalSetting.Get(V5Contract, false));
    }

    private static void SetContractsState(bool state)
    {
        LocalSetting.Set(V1Contract, state);
        LocalSetting.Set(V2Contract, state);
        LocalSetting.Set(V3Contract, state);
        LocalSetting.Set(V4Contract, state);
        LocalSetting.Set(V5Contract, state);
    }
}