// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.ExceptionService;

internal sealed class HutaoException : Exception
{
    public HutaoException(HutaoExceptionKind kind, string message, Exception? innerException)
        : this(message, innerException)
    {
        Kind = kind;
    }

    private HutaoException(string message, Exception? innerException)
        : base($"{message}\n{innerException?.Message}", innerException)
    {
    }

    public HutaoExceptionKind Kind { get; private set; }

    [DoesNotReturn]
    public static HutaoException Throw(HutaoExceptionKind kind, string message, Exception? innerException = default)
    {
        throw new HutaoException(kind, message, innerException);
    }

    public static void ThrowIf(bool condition, HutaoExceptionKind kind, string message, Exception? innerException = default)
    {
        if (condition)
        {
            throw new HutaoException(kind, message, innerException);
        }
    }

    public static void ThrowIfNot(bool condition, HutaoExceptionKind kind, string message, Exception? innerException = default)
    {
        if (!condition)
        {
            throw new HutaoException(kind, message, innerException);
        }
    }

    [DoesNotReturn]
    public static HutaoException GachaStatisticsInvalidItemId(uint id, Exception? innerException = default)
    {
        string message = SH.FormatServiceGachaStatisticsFactoryItemIdInvalid(id);
        throw new HutaoException(HutaoExceptionKind.GachaStatisticsInvalidItemId, message, innerException);
    }

    [DoesNotReturn]
    public static HutaoException UserdataCorrupted(string message, Exception? innerException = default)
    {
        throw new HutaoException(HutaoExceptionKind.UserdataCorrupted, message, innerException);
    }

    [DoesNotReturn]
    public static InvalidCastException InvalidCast<TFrom, TTo>(string name, Exception? innerException = default)
    {
        string message = $"This instance of '{typeof(TFrom).FullName}' '{name}' doesn't implement '{typeof(TTo).FullName}'";
        throw new InvalidCastException(message, innerException);
    }

    [DoesNotReturn]
    public static OperationCanceledException OperationCanceled(string message, Exception? innerException = default)
    {
        return new OperationCanceledException(message, innerException);
    }
}