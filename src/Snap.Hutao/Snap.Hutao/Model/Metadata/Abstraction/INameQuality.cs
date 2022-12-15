// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;

namespace Snap.Hutao.Model.Metadata.Abstraction;

/// <summary>
/// 物品与星级
/// </summary>
public interface INameQuality
{
    /// <summary>
    /// 名称
    /// </summary>
    string Name { get; }

    /// <summary>
    /// 星级
    /// </summary>
    ItemQuality Quality { get; }
}

