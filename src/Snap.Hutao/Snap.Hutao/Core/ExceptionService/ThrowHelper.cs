// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.CompilerServices;

namespace Snap.Hutao.Core.ExceptionService;

/// <summary>
/// 帮助更好的抛出异常
/// </summary>
[HighQuality]
[System.Diagnostics.StackTraceHidden]
[Obsolete("Use HutaoException instead")]
internal static class ThrowHelper
{
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static ArgumentException Argument(string message, string? paramName)
    {
        throw new ArgumentException(message, paramName);
    }
}