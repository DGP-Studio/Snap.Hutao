﻿// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;

namespace Snap.Hutao.Message;

/// <summary>
/// 成就存档切换消息
/// </summary>
[HighQuality]
[Obsolete]
internal sealed class AchievementArchiveChangedMessage : ValueChangedMessage<AchievementArchive>
{
}