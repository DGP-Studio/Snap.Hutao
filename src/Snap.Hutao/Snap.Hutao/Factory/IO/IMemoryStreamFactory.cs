// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.IO;

namespace Snap.Hutao.Factory.IO;

internal interface IMemoryStreamFactory
{
    MemoryStream GetStream();

    MemoryStream GetStreamExactly(long requiredSize);
}