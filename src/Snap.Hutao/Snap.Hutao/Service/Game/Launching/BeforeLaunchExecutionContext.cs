// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Game.FileSystem;
using Snap.Hutao.Service.Game.Package;
using Snap.Hutao.Service.Game.Scheme;
using Snap.Hutao.ViewModel.Game;
using System.Collections.Concurrent;

namespace Snap.Hutao.Service.Game.Launching;

internal sealed class BeforeLaunchExecutionContext
{
    private readonly ConcurrentDictionary<string, bool> options = [];

    public required IViewModelSupportLaunchExecution2 ViewModel { get; init; }

    public required IServiceProvider ServiceProvider { get; init; }

    public required ITaskContext TaskContext { get; init; }

    public required IGameFileSystem FileSystem { get; set; }

    public required IHoyoPlayService HoyoPlay { get; init; }

    public required IMessenger Messenger { get; init; }

    public required LaunchOptions LaunchOptions { get; init; }

    public required LaunchScheme CurrentScheme { get; init; }

    public required LaunchScheme TargetScheme { get; init; }

    public bool this[string key]
    {
        get => options[key];
        set => options[key] = value;
    }
}