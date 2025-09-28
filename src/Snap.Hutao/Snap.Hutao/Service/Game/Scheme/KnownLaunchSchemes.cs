// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;
using System.Collections.Immutable;

namespace Snap.Hutao.Service.Game.Scheme;

internal static class KnownLaunchSchemes
{
    private static readonly LaunchScheme ServerChineseChannel00SubChannel00Compat = new LaunchSchemeChinese(ChannelType.Default, SubChannelType.Default, false);
    private static readonly LaunchScheme ServerChineseChannel00SubChannel01Compat = new LaunchSchemeChinese(ChannelType.Default, SubChannelType.Official, false);
    private static readonly LaunchScheme ServerChineseChannel01SubChannel00 = new LaunchSchemeChinese(ChannelType.Official, SubChannelType.Default);
    private static readonly LaunchScheme ServerChineseChannel01SubChannel01 = new LaunchSchemeChinese(ChannelType.Official, SubChannelType.Official);
    private static readonly LaunchScheme ServerChineseChannel01SubChannel02 = new LaunchSchemeChinese(ChannelType.Official, SubChannelType.NoTapTap);
    private static readonly LaunchScheme ServerChineseChannel01SubChannel03Compat = new LaunchSchemeChinese(ChannelType.Official, SubChannelType.Epic, false);
    private static readonly LaunchScheme ServerChineseChannel01SubChannel06Compat = new LaunchSchemeChinese(ChannelType.Official, SubChannelType.Google, false);
    private static readonly LaunchScheme ServerChineseChannel01SubChannel14Compat = new LaunchSchemeChinese(ChannelType.Official, (SubChannelType)14, false);
    private static readonly LaunchScheme ServerChineseChannel02SubChannel01Compat = new LaunchSchemeChinese(ChannelType.MiHoYoSONY, SubChannelType.Official, false);

    private static readonly LaunchScheme ServerChineseChannel14SubChannel00 = new LaunchSchemeBilibili(SubChannelType.Default);
    private static readonly LaunchScheme ServerChineseChannel14SubChannel01Compat = new LaunchSchemeBilibili(SubChannelType.Official, false);
    private static readonly LaunchScheme ServerChineseChannel14SubChannel02Compat = new LaunchSchemeBilibili(SubChannelType.NoTapTap, false);
    private static readonly LaunchScheme ServerChineseChannel14SubChannel06Compat = new LaunchSchemeBilibili(SubChannelType.Google, false);
    private static readonly LaunchScheme ServerChineseChannel14SubChannel14Compat = new LaunchSchemeBilibili((SubChannelType)14, false);
    private static readonly LaunchScheme ServerChineseChannel14SubChannel16Compat = new LaunchSchemeBilibili((SubChannelType)16, false);

    private static readonly LaunchScheme ServerOverseaChannel00SubChannel00Compat = new LaunchSchemeOversea(ChannelType.Default, SubChannelType.Default, false);
    private static readonly LaunchScheme ServerOverseaChannel01SubChannel00 = new LaunchSchemeOversea(ChannelType.Official, SubChannelType.Default);
    private static readonly LaunchScheme ServerOverseaChannel01SubChannel01 = new LaunchSchemeOversea(ChannelType.Official, SubChannelType.Official);
    private static readonly LaunchScheme ServerOverseaChannel01SubChannel02Compat = new LaunchSchemeOversea(ChannelType.Official, SubChannelType.NoTapTap, false);
    private static readonly LaunchScheme ServerOverseaChannel01SubChannel03 = new LaunchSchemeOversea(ChannelType.Official, SubChannelType.Epic);
    private static readonly LaunchScheme ServerOverseaChannel01SubChannel06 = new LaunchSchemeOversea(ChannelType.Official, SubChannelType.Google);
    private static readonly LaunchScheme ServerOverseaChannel02SubChannel01Compat = new LaunchSchemeOversea(ChannelType.MiHoYoSONY, SubChannelType.Official, false);
    private static readonly LaunchScheme ServerOverseaChannel14SubChannel00Compat = new LaunchSchemeOversea(ChannelType.Bili, SubChannelType.Default, false);
    private static readonly LaunchScheme ServerOverseaChannel14SubChannel14Compat = new LaunchSchemeOversea(ChannelType.Bili, (SubChannelType)14, false);

    public static ImmutableArray<LaunchScheme> Values { get; } =
    [
        ServerChineseChannel00SubChannel00Compat,
        ServerChineseChannel00SubChannel01Compat,
        ServerChineseChannel01SubChannel00,
        ServerChineseChannel01SubChannel01,
        ServerChineseChannel01SubChannel02,
        ServerChineseChannel01SubChannel03Compat,
        ServerChineseChannel01SubChannel06Compat,
        ServerChineseChannel01SubChannel14Compat,
        ServerChineseChannel02SubChannel01Compat,

        ServerChineseChannel14SubChannel00,
        ServerChineseChannel14SubChannel01Compat,
        ServerChineseChannel14SubChannel02Compat,
        ServerChineseChannel14SubChannel06Compat,
        ServerChineseChannel14SubChannel14Compat,
        ServerChineseChannel14SubChannel16Compat,

        ServerOverseaChannel00SubChannel00Compat,
        ServerOverseaChannel01SubChannel00,
        ServerOverseaChannel01SubChannel01,
        ServerOverseaChannel01SubChannel02Compat,
        ServerOverseaChannel01SubChannel03,
        ServerOverseaChannel01SubChannel06,
        ServerOverseaChannel02SubChannel01Compat,
        ServerOverseaChannel14SubChannel00Compat,
        ServerOverseaChannel14SubChannel14Compat,
    ];

    public static ImmutableArray<LaunchScheme> BetaValues { get; } =
    [
        ServerChineseChannel01SubChannel01,
        ServerOverseaChannel01SubChannel00,
    ];

    public static IEnumerable<LaunchScheme> EnumerateNotCompatOnly(bool isOversea)
    {
        return Values.Where(scheme => scheme.IsNotCompatOnly && scheme.IsOversea == isOversea);
    }
}