// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Context.FileSystem.Location;

namespace Snap.Hutao.Context.FileSystem;

/// <summary>
/// 我的文档上下文
/// </summary>
[Injection(InjectAs.Transient)]
internal class MyDocumentContext : FileSystemContext
{
    /// <inheritdoc cref="FileSystemContext"/>
    public MyDocumentContext(MyDocument myDocument)
        : base(myDocument)
    {
    }
}