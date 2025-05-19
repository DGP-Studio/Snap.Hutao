// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;
using System.Collections.Immutable;

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
    private static readonly LaunchScheme ServerChineseChannelBilibiliSubChannelGoogleCompat = new LaunchSchemeBilibili(SubChannelType.Google, false);
    private static readonly LaunchScheme ServerChineseChannelBilibiliSubChannel14Compat = new LaunchSchemeBilibili((SubChannelType)14, false);

    private static readonly LaunchScheme ServerGlobalChannelDefaultSubChannelDefaultCompat = new LaunchSchemeOversea(ChannelType.Default, SubChannelType.Default, false);
    private static readonly LaunchScheme ServerGlobalChannelOfficialSubChannelDefault = new LaunchSchemeOversea(ChannelType.Official, SubChannelType.Default);
    private static readonly LaunchScheme ServerGlobalChannelOfficialSubChannelOfficial = new LaunchSchemeOversea(ChannelType.Official, SubChannelType.Official);
    private static readonly LaunchScheme ServerGlobalChannelOfficialSubChannelEpic = new LaunchSchemeOversea(ChannelType.Official, SubChannelType.Epic);
    private static readonly LaunchScheme ServerGlobalChannelOfficialSubChannelGoogle = new LaunchSchemeOversea(ChannelType.Official, SubChannelType.Google);
    private static readonly LaunchScheme ServerGlobalChannelBilibiliSubChannel14Compat = new LaunchSchemeOversea(ChannelType.Bili, (SubChannelType)14, false);

    public static ImmutableArray<LaunchScheme> Values { get; } =
    [
        ServerChineseChannelDefaultSubChannelDefaultCompat,
        ServerChineseChannelOfficialSubChannelDefault,
        ServerChineseChannelOfficialSubChannelOfficial,
        ServerChineseChannelOfficialSubChannelNoTapTap,
        ServerChineseChannelOfficialSubChannelEpicCompat,

        ServerChineseChannelBilibiliSubChannelDefault,
        ServerChineseChannelBilibiliSubChannelOfficialCompat,
        ServerChineseChannelBilibiliSubChannelGoogleCompat,
        ServerChineseChannelBilibiliSubChannel14Compat,

        ServerGlobalChannelDefaultSubChannelDefaultCompat,
        ServerGlobalChannelOfficialSubChannelDefault,
        ServerGlobalChannelOfficialSubChannelOfficial,
        ServerGlobalChannelOfficialSubChannelEpic,
        ServerGlobalChannelOfficialSubChannelGoogle,
        ServerGlobalChannelBilibiliSubChannel14Compat,
    ];
}