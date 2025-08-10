// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.ExceptionService;

internal readonly struct CapturedException
{
    public readonly SentryId Id;
    public readonly Exception Exception;

    public CapturedException(SentryId id, Exception exception)
    {
        Id = id;
        Exception = exception;
    }
}