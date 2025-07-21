// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Model.Calculable;

internal interface ICalculablePromoteLevel : ICalculableMinMaxLevel
{
    PromoteLevel PromoteLevel { get; }

    bool IsPromoted { get; }

    bool IsPromotionAvailable { get; }
}