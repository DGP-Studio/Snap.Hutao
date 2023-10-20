// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;

namespace Snap.Hutao.Service.Game;

/// <summary>
/// 方案列表部分
/// </summary>
internal sealed partial class LaunchScheme
{
    private const int SdkStaticLauncherChineseId = 18;
    private const int SdkStaticLauncherBilibiliId = 17;
    private const int SdkStaticLauncherGlobalId = 10;

    private const string SdkStaticLauncherChineseKey = "eYd89JmJ";
    private const string SdkStaticLauncherBilibiliKey = "KAtdSsoQ";
    private const string SdkStaticLauncherGlobalKey = "gcStgarh";

    private static readonly LaunchScheme ServerChineseChannelDefaultSubChannelDefaultCompatOnly = new()
    {
        LauncherId = SdkStaticLauncherChineseId,
        Key = SdkStaticLauncherChineseKey,
        Channel = ChannelType.Default,
        SubChannel = SubChannelType.Default,
        IsOversea = false,
        IsNotCompatOnly = false,
    };

    private static readonly LaunchScheme ServerChineseChannelOfficialSubChannelDefault = new()
    {
        LauncherId = SdkStaticLauncherChineseId,
        Key = SdkStaticLauncherChineseKey,
        Channel = ChannelType.Official,
        SubChannel = SubChannelType.Default,
        IsOversea = false,
    };

    private static readonly LaunchScheme ServerChineseChannelOfficialSubChannelOfficial = new()
    {
        LauncherId = SdkStaticLauncherChineseId,
        Key = SdkStaticLauncherChineseKey,
        Channel = ChannelType.Official,
        SubChannel = SubChannelType.Official,
        IsOversea = false,
    };

    private static readonly LaunchScheme ServerChineseChannelOfficialSubChannelNoTapTap = new()
    {
        LauncherId = SdkStaticLauncherChineseId,
        Key = SdkStaticLauncherChineseKey,
        Channel = ChannelType.Official,
        SubChannel = SubChannelType.NoTapTap,
        IsOversea = false,
    };

    private static readonly LaunchScheme ServerChineseChannelOfficialSubChannelEpicCompatOnly = new()
    {
        LauncherId = SdkStaticLauncherChineseId,
        Key = SdkStaticLauncherChineseKey,
        Channel = ChannelType.Official,
        SubChannel = SubChannelType.Epic,
        IsOversea = false,
        IsNotCompatOnly = false,
    };

    private static readonly LaunchScheme ServerChineseChannelBilibiliSubChannelDefault = new()
    {
        LauncherId = SdkStaticLauncherBilibiliId,
        Key = SdkStaticLauncherBilibiliKey,
        Channel = ChannelType.Bili,
        SubChannel = SubChannelType.Default,
        IsOversea = false,
    };

    private static readonly LaunchScheme ServerChineseChannelBilibiliSubChannelOfficialCompatOnly = new()
    {
        LauncherId = SdkStaticLauncherBilibiliId,
        Key = SdkStaticLauncherBilibiliKey,
        Channel = ChannelType.Bili,
        SubChannel = SubChannelType.Official,
        IsOversea = false,
        IsNotCompatOnly = false,
    };

    private static readonly LaunchScheme ServerGlobalChannelDefaultSubChannelDefaultCompatOnly = new()
    {
        LauncherId = SdkStaticLauncherGlobalId,
        Key = SdkStaticLauncherGlobalKey,
        Channel = ChannelType.Default,
        SubChannel = SubChannelType.Default,
        IsOversea = true,
        IsNotCompatOnly = false,
    };

    private static readonly LaunchScheme ServerGlobalChannelOfficialSubChannelDefault = new()
    {
        LauncherId = SdkStaticLauncherGlobalId,
        Key = SdkStaticLauncherGlobalKey,
        Channel = ChannelType.Official,
        SubChannel = SubChannelType.Default,
        IsOversea = true,
    };

    private static readonly LaunchScheme ServerGlobalChannelOfficialSubChannelOfficial = new()
    {
        LauncherId = SdkStaticLauncherGlobalId,
        Key = SdkStaticLauncherGlobalKey,
        Channel = ChannelType.Official,
        SubChannel = SubChannelType.Official,
        IsOversea = true,
    };

    private static readonly LaunchScheme ServerGlobalChannelOfficialSubChannelEpic = new()
    {
        LauncherId = SdkStaticLauncherGlobalId,
        Key = SdkStaticLauncherGlobalKey,
        Channel = ChannelType.Official,
        SubChannel = SubChannelType.Epic,
        IsOversea = true,
    };

    private static readonly LaunchScheme ServerGlobalChannelOfficialSubChannelGoogle = new()
    {
        LauncherId = SdkStaticLauncherGlobalId,
        Key = SdkStaticLauncherGlobalKey,
        Channel = ChannelType.Official,
        SubChannel = SubChannelType.Google,
        IsOversea = true,
    };

    /// <summary>
    /// 获取已知的启动方案
    /// </summary>
    /// <returns>已知的启动方案</returns>
    public static List<LaunchScheme> GetKnownSchemes()
    {
        return new List<LaunchScheme>()
        {
            // 官服
            ServerChineseChannelDefaultSubChannelDefaultCompatOnly,
            ServerChineseChannelOfficialSubChannelDefault,
            ServerChineseChannelOfficialSubChannelOfficial,
            ServerChineseChannelOfficialSubChannelNoTapTap,
            ServerChineseChannelOfficialSubChannelEpicCompatOnly,

            // 渠道服
            ServerChineseChannelBilibiliSubChannelDefault,
            ServerChineseChannelBilibiliSubChannelOfficialCompatOnly,

            // 国际服
            ServerGlobalChannelDefaultSubChannelDefaultCompatOnly,
            ServerGlobalChannelOfficialSubChannelDefault,
            ServerGlobalChannelOfficialSubChannelOfficial,
            ServerGlobalChannelOfficialSubChannelEpic,
            ServerGlobalChannelOfficialSubChannelGoogle,
        };
    }
}