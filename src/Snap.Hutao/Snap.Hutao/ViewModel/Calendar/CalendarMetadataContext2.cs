// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Metadata.Avatar;
using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.ViewModel.Calendar;

internal sealed class CalendarMetadataContext2
{
    public required CalendarMetadataContext MetadataContext { get; init; }

    public required ILookup<MonthAndDay, Avatar> AvatarBirthdays { get; init; }

    public required ILookup<MaterialId, CalendarItem> MaterialItems { get; init; }
}