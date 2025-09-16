// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Diagnostics;

namespace Snap.Hutao.Service.Game.Launching.Context;

internal readonly struct LaunchExecutionOptionsKey<TValue>
{
    public LaunchExecutionOptionsKey(string key)
    {
        Key = key;
    }

    public string Key { get; }
}

internal sealed class LaunchExecutionOptionsKey
{
    public static readonly LaunchExecutionOptionsKey<bool> ChannelOptionsChanged = new(nameof(ChannelOptionsChanged));
    public static readonly LaunchExecutionOptionsKey<string> LoginAuthTicket = new(nameof(LoginAuthTicket));
    public static readonly LaunchExecutionOptionsKey<IProcess> RunningProcess = new(nameof(RunningProcess));
}