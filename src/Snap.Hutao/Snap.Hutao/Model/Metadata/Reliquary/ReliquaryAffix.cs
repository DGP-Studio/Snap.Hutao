// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Model.Metadata.Reliquary;

/// <summary>
/// 圣遗物突破属性
/// </summary>
[HighQuality]
internal sealed class ReliquaryAffix : ReliquaryMainAffix
{
    /// <summary>
    /// Id
    /// </summary>
    public new ReliquaryAffixId Id { get; set; }

    /// <summary>
    /// 值
    /// </summary>
    public float Value { get; set; }
}