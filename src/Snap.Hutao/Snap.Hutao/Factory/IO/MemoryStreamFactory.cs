// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.IO;
using System.Diagnostics;
using System.IO;

namespace Snap.Hutao.Factory.IO;

[Injection(InjectAs.Singleton, typeof(IMemoryStreamFactory))]
internal sealed class MemoryStreamFactory : IMemoryStreamFactory
{
    private readonly RecyclableMemoryStreamManager manager;

    public MemoryStreamFactory()
    {
        RecyclableMemoryStreamManager.Options options = new()
        {
            BlockSize = 256 * 1024, // 256KB
            LargeBufferMultiple = 1024 * 1024, // 1MB
            MaximumBufferSize = 32 * 1024 * 1024, // 32MB
            MaximumSmallPoolFreeBytes = 16 * 1024 * 1024, // 16MB
            MaximumLargePoolFreeBytes = 128 * 1024 * 1024, // 128MB
        };

        manager = new(options);
#if DEBUG
        manager.StreamDoubleDisposed += (s, e) =>
        {
            Debugger.Break();
        };
#endif
    }

    public MemoryStream GetStream()
    {
        return manager.GetStream();
    }
}