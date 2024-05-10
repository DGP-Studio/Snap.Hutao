// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.InteropServices;

namespace Snap.Hutao.Win32;

internal readonly struct UnmanagedAccess<T> : IDisposable
    where T : class
{
    private readonly nint handle;

    public UnmanagedAccess(T value)
    {
        handle = GCHandle.ToIntPtr(GCHandle.Alloc(value));
    }

    public static implicit operator nint(UnmanagedAccess<T> access)
    {
        return access.handle;
    }

    public static implicit operator nuint(UnmanagedAccess<T> access)
    {
        return (nuint)access.handle;
    }

    public void Dispose()
    {
        GCHandle.FromIntPtr(handle).Free();
    }
}

internal static class UnmanagedAccess
{
    public static UnmanagedAccess<T> Create<T>(T value)
        where T : class
    {
        return new UnmanagedAccess<T>(value);
    }

    public static T? Get<T>(nint handle)
        where T : class
    {
        return GCHandle.FromIntPtr(handle).Target as T;
    }

    public static T? Get<T>(nuint handle)
        where T : class
    {
        return GCHandle.FromIntPtr((nint)handle).Target as T;
    }
}