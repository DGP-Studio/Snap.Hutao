// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Model.Metadata.Reliquary;

/// <summary>
/// 圣遗物主属性
/// </summary>
public class ReliquaryAffixBase
{
    /// <summary>
    /// Id
    /// </summary>
    public ReliquaryMainAffixId Id { get; set; }

    /// <summary>
    /// 战斗属性
    /// </summary>
    public FightProperty Type { get; set; }
}