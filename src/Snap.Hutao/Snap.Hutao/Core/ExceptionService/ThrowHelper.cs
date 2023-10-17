// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Game;
using Snap.Hutao.Service.Game.Package;
using System.Runtime.CompilerServices;

namespace Snap.Hutao.Core.ExceptionService;

/// <summary>
/// 帮助更好的抛出异常
/// </summary>
[HighQuality]
[System.Diagnostics.StackTraceHidden]
internal static class ThrowHelper
{
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static ArgumentException Argument(string message, string? paramName)
    {
        throw new ArgumentException(message, paramName);
    }

    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static DatabaseCorruptedException DatabaseCorrupted(string message, Exception? inner)
    {
        throw new DatabaseCorruptedException(message, inner);
    }

    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static GameFileOperationException GameFileOperation(string message, Exception? inner)
    {
        throw new GameFileOperationException(message, inner);
    }

    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static InvalidOperationException InvalidOperation(string message, Exception? inner = default)
    {
        throw new InvalidOperationException(message, inner);
    }

    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static NotSupportedException NotSupported()
    {
        throw new NotSupportedException();
    }

    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static OperationCanceledException OperationCanceled(string message, Exception? inner = default)
    {
        throw new OperationCanceledException(message, inner);
    }

    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static PackageConvertException PackageConvert(string message, Exception? inner)
    {
        throw new PackageConvertException(message, inner);
    }

    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static RuntimeEnvironmentException RuntimeEnvironment(string message, Exception? inner)
    {
        throw new RuntimeEnvironmentException(message, inner);
    }

    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static UserdataCorruptedException UserdataCorrupted(string message, Exception? inner)
    {
        throw new UserdataCorruptedException(message, inner);
    }
}