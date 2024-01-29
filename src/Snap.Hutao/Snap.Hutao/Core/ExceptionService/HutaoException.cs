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

    public static void ThrowIf(bool condition, HutaoExceptionKind kind, string message, Exception? innerException = default)
    {
        if (condition)
        {
            throw new HutaoException(kind, message, innerException);
        }
    }
}