// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;

namespace Snap.Hutao.Service.Game.Scheme;

internal static class KnownLaunchSchemes
{
    private static readonly LaunchScheme ServerChineseChannelDefaultSubChannelDefaultCompat = new LaunchSchemeChinese(ChannelType.Default, SubChannelType.Default, false);
    private static readonly LaunchScheme ServerChineseChannelOfficialSubChannelDefault = new LaunchSchemeChinese(ChannelType.Official, SubChannelType.Default);
    private static readonly LaunchScheme ServerChineseChannelOfficialSubChannelOfficial = new LaunchSchemeChinese(ChannelType.Official, SubChannelType.Official);
    private static readonly LaunchScheme ServerChineseChannelOfficialSubChannelNoTapTap = new LaunchSchemeChinese(ChannelType.Official, SubChannelType.NoTapTap);
    private static readonly LaunchScheme ServerChineseChannelOfficialSubChannelEpicCompat = new LaunchSchemeChinese(ChannelType.Official, SubChannelType.Epic, false);

    private static readonly LaunchScheme ServerChineseChannelBilibiliSubChannelDefault = new LaunchSchemeBilibili(SubChannelType.Default);
    private static readonly LaunchScheme ServerChineseChannelBilibiliSubChannelOfficialCompat = new LaunchSchemeBilibili(SubChannelType.Official, false);

    private static readonly LaunchScheme ServerGlobalChannelDefaultSubChannelDefaultCompat = new LaunchSchemeOversea(ChannelType.Default, SubChannelType.Default, false);
    private static readonly LaunchScheme ServerGlobalChannelOfficialSubChannelDefault = new LaunchSchemeOversea(ChannelType.Official, SubChannelType.Default);
    private static readonly LaunchScheme ServerGlobalChannelOfficialSubChannelOfficial = new LaunchSchemeOversea(ChannelType.Official, SubChannelType.Official);
    private static readonly LaunchScheme ServerGlobalChannelOfficialSubChannelEpic = new LaunchSchemeOversea(ChannelType.Official, SubChannelType.Epic);
    private static readonly LaunchScheme ServerGlobalChannelOfficialSubChannelGoogle = new LaunchSchemeOversea(ChannelType.Official, SubChannelType.Google);

    /// <summary>
    /// 获取已知的启动方案
    /// </summary>
    /// <returns>已知的启动方案</returns>
    public static List<LaunchScheme> Get()
    {
        return new List<LaunchScheme>()
        {
            // 官服
            ServerChineseChannelDefaultSubChannelDefaultCompat,
            ServerChineseChannelOfficialSubChannelDefault,
            ServerChineseChannelOfficialSubChannelOfficial,
            ServerChineseChannelOfficialSubChannelNoTapTap,
            ServerChineseChannelOfficialSubChannelEpicCompat,

            // 渠道服
            ServerChineseChannelBilibiliSubChannelDefault,
            ServerChineseChannelBilibiliSubChannelOfficialCompat,

            // 国际服
            ServerGlobalChannelDefaultSubChannelDefaultCompat,
            ServerGlobalChannelOfficialSubChannelDefault,
            ServerGlobalChannelOfficialSubChannelOfficial,
            ServerGlobalChannelOfficialSubChannelEpic,
            ServerGlobalChannelOfficialSubChannelGoogle,
        };
    }
}