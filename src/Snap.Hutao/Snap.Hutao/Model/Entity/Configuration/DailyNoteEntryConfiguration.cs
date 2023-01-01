// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Snap.Hutao.Model.Entity.Configuration;

/// <summary>
/// 实时便笺入口配置
/// </summary>
internal class DailyNoteEntryConfiguration : IEntityTypeConfiguration<DailyNoteEntry>
{
    /// <inheritdoc/>
    public void Configure(EntityTypeBuilder<DailyNoteEntry> builder)
    {
        builder.Property(e => e.DailyNote)
            .HasColumnType("TEXT")
            .HasConversion<JsonTextValueConverter<Web.Hoyolab.Takumi.GameRecord.DailyNote.DailyNote>>();
    }
}
