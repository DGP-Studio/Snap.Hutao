// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Service.Metadata.ContextAbstraction;

internal interface IMetadataDictionaryIdAchievementSource
{
    Dictionary<AchievementId, Model.Metadata.Achievement.Achievement> IdAchievementMap { get; set; }
}