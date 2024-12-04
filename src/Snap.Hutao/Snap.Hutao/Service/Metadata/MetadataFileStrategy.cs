// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Metadata;

internal sealed class MetadataFileStrategy
{
    public MetadataFileStrategy(string fileName, bool isScattered = false)
    {
        Name = fileName;
        IsScattered = isScattered;
    }

    public string Name { get; }

    public bool IsScattered { get; }
}