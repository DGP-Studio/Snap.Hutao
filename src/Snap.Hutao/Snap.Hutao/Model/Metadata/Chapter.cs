// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Model.Metadata;

internal sealed class Chapter
{
    public ChapterId Id { get; set; }

    public ChapterGroupId GroupId { get; set; }

    public QuestId BeginQuestId { get; set; }

    public QuestId EndQuestId { get; set; }

    public uint NeedPlayerLevel { get; set; }

    public string Number { get; set; } = default!;

    public string Title { get; set; } = default!;

    public string Icon { get; set; } = default!;

    public string ImageTitle { get; set; } = default!;

    public string SerialNumberIcon { get; set; } = default!;

    public City CityId { get; set; }

    public QuestType QuestType { get; set; }
}
