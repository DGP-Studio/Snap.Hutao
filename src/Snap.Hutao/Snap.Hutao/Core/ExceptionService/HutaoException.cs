// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Snap.Hutao.Core.ExceptionService;

internal sealed class HutaoException : Exception
{
    [StackTraceHidden]
    public HutaoException(string message, Exception? innerException)
        : base($"{message}\n{innerException?.Message}", innerException)
    {
    }

    /// <summary>
    /// A unique exception instance used to mark an exception.
    /// </summary>
    public static HutaoException Marker { get; } = new("MARKER EXCEPTION", null);

    public static bool ContainsMarker(Exception? exception)
    {
        while (true)
        {
            switch (exception)
            {
                case null:
                    return false;
                case HutaoException hutaoException:
                    return ReferenceEquals(hutaoException, Marker);
                case AggregateException aggregateException when aggregateException.InnerExceptions.Any(ContainsMarker):
                    return true;
                default:
                    exception = exception.InnerException;
                    break;
            }
        }
    }

    [StackTraceHidden]
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static HutaoException Throw(string message, Exception? innerException = default)
    {
        throw new HutaoException(message, innerException);
    }

    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ThrowIf([DoesNotReturnIf(true)] bool condition, string message, Exception? innerException = default)
    {
        if (condition)
        {
            throw new HutaoException(message, innerException);
        }
    }

    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ThrowIfNot([DoesNotReturnIf(false)] bool condition, string message, Exception? innerException = default)
    {
        if (!condition)
        {
            throw new HutaoException(message, innerException);
        }
    }

    [StackTraceHidden]
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static ArgumentException Argument(string message, string? paramName)
    {
        throw new ArgumentException(message, paramName);
    }

    [StackTraceHidden]
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static HutaoException GachaStatisticsInvalidItemId(uint id, Exception? innerException = default)
    {
        throw new HutaoException(SH.FormatServiceGachaStatisticsFactoryItemIdInvalid(id), innerException);
    }

    [StackTraceHidden]
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static InvalidCastException InvalidCast<TFrom, TTo>(string name, Exception? innerException = default)
    {
        string message = $"This instance of '{typeof(TFrom).FullName}' '{name}' doesn't implement '{typeof(TTo).FullName}'";
        throw new InvalidCastException(message, innerException);
    }

    [StackTraceHidden]
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static InvalidOperationException InvalidOperation(string message, Exception? innerException = default)
    {
        throw new InvalidOperationException(message, innerException);
    }

    [StackTraceHidden]
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static NotSupportedException NotSupported(string? message = default, Exception? innerException = default)
    {
        throw new NotSupportedException(message, innerException);
    }

    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void NotSupportedIf([DoesNotReturnIf(true)] bool condition, string? message = default, Exception? innerException = default)
    {
        if (condition)
        {
            throw new NotSupportedException(message, innerException);
        }
    }

    [StackTraceHidden]
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static OperationCanceledException OperationCanceled(string message, Exception? innerException = default)
    {
        throw new OperationCanceledException(message, innerException);
    }

    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void OperationCanceledIf([DoesNotReturnIf(true)] bool condition, string message, Exception? innerException = default)
    {
        if (condition)
        {
            throw new OperationCanceledException(message, innerException);
        }
    }

    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void OperationCanceledIf(IServiceScopeIsDisposed serviceScopeIsDisposed, string message, Exception? innerException = default)
    {
        if (serviceScopeIsDisposed.IsDisposed)
        {
            throw new OperationCanceledException(message, innerException);
        }
    }
}