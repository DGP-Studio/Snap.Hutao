// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Context.FileSystem.Location;

namespace Snap.Hutao.Context.FileSystem;

/// <summary>
/// 元数据上下文
/// </summary>
[Injection(InjectAs.Transient)]
internal class MetadataContext : FileSystemContext
{
    /// <inheritdoc cref="FileSystemContext"/>
    public MetadataContext(Metadata metadata)
        : base(metadata)
    {
    }
}