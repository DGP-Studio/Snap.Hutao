// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Context.FileSystem;

/// <summary>
/// 我的文档上下文
/// </summary>
[Injection(InjectAs.Transient)]
internal class HutaoContext : FileSystemContext
{
    /// <inheritdoc cref="FileSystemContext"/>
    public HutaoContext(Location.HutaoLocation myDocument)
        : base(myDocument)
    {
    }
}