// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Numerics;

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

    public static HutaoException ServiceTypeCastFailed<TFrom, TTo>(string name, Exception? innerException = default)
    {
        string message = $"This instance of '{typeof(TFrom).FullName}' '{name}' doesn't implement '{typeof(TTo).FullName}'";
        throw new HutaoException(HutaoExceptionKind.ServiceTypeCastFailed, message, innerException);
    }

    public static HutaoException GachaStatisticsInvalidItemId(uint id, Exception? innerException = default)
    {
        string message = SH.FormatServiceGachaStatisticsFactoryItemIdInvalid(id);
        throw new HutaoException(HutaoExceptionKind.GachaStatisticsInvalidItemId, message, innerException);
    }
}