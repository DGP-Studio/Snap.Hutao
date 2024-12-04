// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Snap.Hutao.Model.Entity.Configuration;

internal sealed class DailyNoteEntryConfiguration : IEntityTypeConfiguration<DailyNoteEntry>
{
    public void Configure(EntityTypeBuilder<DailyNoteEntry> builder)
    {
        builder.Property(e => e.DailyNote)
            .HasColumnType(SqliteTypeNames.Text)
            .HasConversion<JsonTextValueConverter<Web.Hoyolab.Takumi.GameRecord.DailyNote.DailyNote>>();
    }
}
