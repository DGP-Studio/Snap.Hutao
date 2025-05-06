// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao;

// Place to store critical application state
internal static class XamlApplicationLifetime
{
    public static bool DispatcherQueueInitialized { get => Volatile.Read(ref field); set => Volatile.Write(ref field, value); }

    public static bool CultureInfoInitialized { get => Volatile.Read(ref field); set => Volatile.Write(ref field, value); }

    public static bool NotifyIconCreated { get => Volatile.Read(ref field); set => Volatile.Write(ref field, value); }

    public static bool ActivationAndInitializationCompleted { get => Volatile.Read(ref field); set => Volatile.Write(ref field, value); }

    public static bool IsFirstRunAfterUpdate { get => Volatile.Read(ref field); set => Volatile.Write(ref field, value); }

    public static bool Exiting { get => Volatile.Read(ref field); set => Volatile.Write(ref field, value); }

    public static bool Exited { get => Volatile.Read(ref field); set => Volatile.Write(ref field, value); }
}