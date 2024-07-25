// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.IO;
using System.IO;

namespace Snap.Hutao.Factory.IO;

[Injection(InjectAs.Singleton, typeof(IMemoryStreamFactory))]
internal sealed class MemoryStreamFactory : IMemoryStreamFactory
{
    private readonly RecyclableMemoryStreamManager manager = new();

    public MemoryStream GetStream()
    {
        return manager.GetStream();
    }
}