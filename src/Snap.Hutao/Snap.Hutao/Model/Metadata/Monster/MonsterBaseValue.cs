// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Model.Metadata.Monster;

/// <summary>
/// 怪物基本属性
/// </summary>
internal sealed class MonsterBaseValue : BaseValue
{
    /// <summary>
    /// 火抗
    /// </summary>
    public float FireSubHurt { get; set; }

    /// <summary>
    /// 草抗
    /// </summary>
    public float GrassSubHurt { get; set; }

    /// <summary>
    /// 水抗
    /// </summary>
    public float WaterSubHurt { get; set; }

    /// <summary>
    /// 雷抗
    /// </summary>
    public float ElecSubHurt { get; set; }

    /// <summary>
    /// 风抗
    /// </summary>
    public float WindSubHurt { get; set; }

    /// <summary>
    /// 冰抗
    /// </summary>
    public float IceSubHurt { get; set; }

    /// <summary>
    /// 岩抗
    /// </summary>
    public float RockSubHurt { get; set; }

    /// <summary>
    /// 物抗
    /// </summary>
    public float PhysicalSubHurt { get; set; }
}