// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.ExceptionService;

namespace Snap.Hutao.Core.Caching;

internal class InternalImageCacheException : Exception, IInternalException
{
    private InternalImageCacheException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    private InternalImageCacheException(string message)
        : base(message)
    {
    }

    public static InternalImageCacheException Throw(string message, Exception innerException)
    {
        throw new InternalImageCacheException(message, innerException);
    }

    public static InternalImageCacheException Throw(string message)
    {
        throw new InternalImageCacheException(message);
    }
}