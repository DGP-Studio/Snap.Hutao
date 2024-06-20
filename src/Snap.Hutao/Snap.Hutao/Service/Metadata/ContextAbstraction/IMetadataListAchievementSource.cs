﻿// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Metadata.ContextAbstraction;

internal interface IMetadataListAchievementSource
{
    List<Model.Metadata.Achievement.Achievement> Achievements { get; set; }
}