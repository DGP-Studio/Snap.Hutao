﻿// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity.Primitive;
using CalculateItem = Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate.Item;

namespace Snap.Hutao.Service.Cultivation;

internal sealed class InputConsumption
{
    public required CultivateType Type { get; init; }

    public required uint ItemId { get; init; }

    public required List<CalculateItem> Items { get; init; }

    public required LevelInformation LevelInformation { get; init; }

    public required ConsumptionSaveStrategyKind Strategy { get; init; }
}