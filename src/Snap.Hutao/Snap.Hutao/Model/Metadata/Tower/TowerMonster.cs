// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Model.Metadata.Tower;

internal sealed class TowerMonster
{
    /// <summary>
    /// 怪物关系Id
    /// </summary>
    public MonsterRelationshipId Id { get; set; }

    /// <summary>
    /// 个数
    /// </summary>
    public uint Count { get; set; }

    /// <summary>
    /// 是否攻击镇石
    /// </summary>
    public bool AttackMonolith { get; set; }

    /// <summary>
    /// 特殊词条
    /// </summary>
    public List<NameDescription>? Affixes { get; set; }
}