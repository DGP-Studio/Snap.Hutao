// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.IO.Http.Sharding;

internal interface IHttpShardCopyWorker<TStatus> : IDisposable
{
    [SuppressMessage("", "SH003")]
    Task CopyAsync(IProgress<TStatus> progress, CancellationToken token = default);
}