// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.IO.Http.Sharding;

internal interface IHttpShard
{
    long Start { get; init; }

    long End { get; set; }

    long BytesRead { get; set; }
}