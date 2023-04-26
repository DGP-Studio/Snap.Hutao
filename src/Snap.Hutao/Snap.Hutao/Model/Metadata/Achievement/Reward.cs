// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Model.Metadata.Achievement;

/// <summary>
/// 奖励
/// </summary>
internal sealed class Reward
{
    /// <summary>
    /// Id
    /// </summary>
    public MaterialId Id { get; set; }

    /// <summary>
    /// 数量
    /// </summary>
    public int Count { get; set; }
}