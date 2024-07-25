// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.IO;
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
            BlockSize = 256 * 1024,
            LargeBufferMultiple = 1024 * 1024,
            MaximumBufferSize = 32 * 1024 * 1024,
            MaximumSmallPoolFreeBytes = 16 * 1024 * 1024,
            MaximumLargePoolFreeBytes = 128 * 1024 * 1024,
        };

        manager = new(options);
    }

    public MemoryStream GetStream()
    {
        return manager.GetStream();
    }
}