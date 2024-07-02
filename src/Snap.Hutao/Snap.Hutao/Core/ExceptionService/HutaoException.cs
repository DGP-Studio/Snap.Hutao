// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.CompilerServices;

namespace Snap.Hutao.Core.ExceptionService;

internal sealed class HutaoException : Exception
{
    public HutaoException(string message, Exception? innerException)
        : base($"{message}\n{innerException?.Message}", innerException)
    {
    }

    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static HutaoException Throw(string message, Exception? innerException = default)
    {
        throw new HutaoException(message, innerException);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ThrowIf([DoesNotReturnIf(true)] bool condition, string message, Exception? innerException = default)
    {
        if (condition)
        {
            throw new HutaoException(message, innerException);
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ThrowIfNot([DoesNotReturnIf(false)] bool condition, string message, Exception? innerException = default)
    {
        if (!condition)
        {
            throw new HutaoException(message, innerException);
        }
    }

    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static ArgumentException Argument(string message, string? paramName)
    {
        throw new ArgumentException(message, paramName);
    }

    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static HutaoException GachaStatisticsInvalidItemId(uint id, Exception? innerException = default)
    {
        throw new HutaoException(SH.FormatServiceGachaStatisticsFactoryItemIdInvalid(id), innerException);
    }

    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static InvalidCastException InvalidCast<TFrom, TTo>(string name, Exception? innerException = default)
    {
        string message = $"This instance of '{typeof(TFrom).FullName}' '{name}' doesn't implement '{typeof(TTo).FullName}'";
        throw new InvalidCastException(message, innerException);
    }

    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static InvalidOperationException InvalidOperation(string message, Exception? innerException = default)
    {
        throw new InvalidOperationException(message, innerException);
    }

    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static NotSupportedException NotSupported(string? message = default, Exception? innerException = default)
    {
        throw new NotSupportedException(message, innerException);
    }

    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static OperationCanceledException OperationCanceled(string message, Exception? innerException = default)
    {
        throw new OperationCanceledException(message, innerException);
    }
}