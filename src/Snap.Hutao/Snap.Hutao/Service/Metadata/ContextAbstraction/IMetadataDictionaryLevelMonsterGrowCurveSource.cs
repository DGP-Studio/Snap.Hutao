﻿// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Service.Metadata.ContextAbstraction;

internal interface IMetadataDictionaryLevelMonsterGrowCurveSource
{
    Dictionary<Level, Dictionary<GrowCurveType, float>> LevelDictionaryMonsterGrowCurveMap { get; set; }
}