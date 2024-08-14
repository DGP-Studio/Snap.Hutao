// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.IO.Http.Sharding;

internal interface IHttpShard
{
    long Start { get; }

    long End { get; }

    long BytesRead { get; set; }

    AsyncReaderWriterLock ReaderWriterLock { get; }
}