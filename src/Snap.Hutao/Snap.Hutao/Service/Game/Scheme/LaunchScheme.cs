﻿// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Service.Game.Configuration;

namespace Snap.Hutao.Service.Game.Scheme;

internal class LaunchScheme : IEquatable<ChannelOptions>
{
    public string DisplayName
    {
        get
        {
            string name = (Channel, IsOversea) switch
            {
                (ChannelType.Bili, false) => SH.ModelBindingLaunchGameLaunchSchemeBilibili,
                (_, false) => SH.ModelBindingLaunchGameLaunchSchemeChinese,
                (_, true) => SH.ModelBindingLaunchGameLaunchSchemeOversea,
            };

            return $"{name} | {Channel} | {SubChannel}";
        }
    }

    public ChannelType Channel { get; private protected set; }

    public SubChannelType SubChannel { get; private protected set; }

    public string LauncherId { get; private protected set; } = default!;

    public string GameId { get; private protected set; } = default!;

    public bool IsOversea { get; private protected set; }

    public bool IsNotCompatOnly { get; private protected set; } = true;

    public static bool ExecutableIsOversea(string gameFileName)
    {
        return gameFileName.ToUpperInvariant() switch
        {
            GameConstants.GenshinImpactFileNameUpper => true,
            GameConstants.YuanShenFileNameUpper => false,
            _ => throw Requires.Fail("Invalid game executable file name：{0}", gameFileName),
        };
    }

    public bool Equals(ChannelOptions other)
    {
        return Channel == other.Channel && SubChannel == other.SubChannel && IsOversea == other.IsOversea;
    }

    public bool ExecutableMatches(string gameFileName)
    {
        return (IsOversea, gameFileName) switch
        {
            (true, GameConstants.GenshinImpactFileName) => true,
            (false, GameConstants.YuanShenFileName) => true,
            _ => false,
        };
    }
}