// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Model.Metadata;

internal class IdCount
{
    /// <summary>
    /// Id
    /// </summary>
    public MaterialId Id { get; set; }

    /// <summary>
    /// 数量
    /// </summary>
    public uint Count { get; set; }
}