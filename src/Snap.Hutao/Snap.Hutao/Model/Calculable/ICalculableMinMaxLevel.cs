// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Model.Calculable;

internal interface ICalculableMinMaxLevel : ICalculable
{
    uint LevelMin { get; }

    uint LevelMax { get; }

    PromoteLevel PromoteLevel { get; }

    bool IsPromoted { get; }

    bool IsPromotionAvailable { get; }
}