// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Snap.Hutao.Model.Entity.Configuration;

/// <summary>
/// 背包圣遗物配置
/// </summary>
[HighQuality]
internal sealed class InventoryReliquaryConfiguration : IEntityTypeConfiguration<InventoryReliquary>
{
    /// <inheritdoc/>
    public void Configure(EntityTypeBuilder<InventoryReliquary> builder)
    {
        builder.Property(e => e.AppendPropIdList)
            .HasColumnType(SqliteTypeNames.Text)
            .HasConversion(
                list => list.ToString(','),
                text => text.Split(',', StringSplitOptions.None).Select(int.Parse).ToList());
    }
}