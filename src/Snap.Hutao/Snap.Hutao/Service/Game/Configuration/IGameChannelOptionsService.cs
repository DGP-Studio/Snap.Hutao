// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Game.Scheme;

namespace Snap.Hutao.Service.Game.Configuration;

internal interface IGameChannelOptionsService
{
    ChannelOptions GetChannelOptions();

    bool SetChannelOptions(LaunchScheme scheme);
}