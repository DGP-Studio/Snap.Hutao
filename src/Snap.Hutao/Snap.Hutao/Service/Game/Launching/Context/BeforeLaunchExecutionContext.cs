// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Game.FileSystem;
using Snap.Hutao.Service.Game.Package;
using Snap.Hutao.Service.Game.Scheme;
using Snap.Hutao.ViewModel.Game;
using System.Collections.Concurrent;

namespace Snap.Hutao.Service.Game.Launching.Context;

internal sealed class BeforeLaunchExecutionContext
{
    private readonly ConcurrentDictionary<string, object?> options = [];

    public required IViewModelSupportLaunchExecution ViewModel { get; init; }

    public required IProgress<LaunchStatus?> Progress { get; init; }

    public required IServiceProvider ServiceProvider { get; init; }

    public required ITaskContext TaskContext { get; init; }

    public required IGameFileSystem FileSystem { get; set; }

    public required IHoyoPlayService HoyoPlay { get; init; }

    public required IMessenger Messenger { get; init; }

    public required LaunchOptions LaunchOptions { get; init; }

    public required LaunchScheme CurrentScheme { get; init; }

    public required LaunchScheme TargetScheme { get; init; }

    public required GameIdentity Identity { get; init; }

    public bool TryGetOption<TValue>(LaunchExecutionOptionsKey<TValue> key, [MaybeNullWhen(false)] out TValue value)
    {
        if (options.TryGetValue(key.Key, out object? objValue) && objValue is TValue tValue)
        {
            value = tValue;
            return true;
        }

        value = default;
        return false;
    }

    public void SetOption<TValue>(LaunchExecutionOptionsKey<TValue> key, TValue value)
    {
        options[key.Key] = value;
    }
}