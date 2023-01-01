// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Snap.Hutao.Model.Entity.Configuration;

/// <summary>
/// 深渊入口配置
/// </summary>
internal class SpiralAbyssEntryConfiguration : IEntityTypeConfiguration<SpiralAbyssEntry>
{
    /// <inheritdoc/>
    public void Configure(EntityTypeBuilder<SpiralAbyssEntry> builder)
    {
        builder.Property(e => e.SpiralAbyss)
            .HasColumnType("TEXT")
            .HasConversion<JsonTextValueConverter<Web.Hoyolab.Takumi.GameRecord.SpiralAbyss.SpiralAbyss>>();
    }
}