// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Snap.Hutao.Model.Entity.Configuration;

/// <summary>
/// 背包圣遗物配置
/// </summary>
internal class InventoryReliquaryConfiguration : IEntityTypeConfiguration<InventoryReliquary>
{
    /// <inheritdoc/>
    public void Configure(EntityTypeBuilder<InventoryReliquary> builder)
    {
        builder.Property(e => e.AppendPropIdList)
            .HasColumnType("TEXT")
            .HasConversion(
                list => string.Join(',', list),
                text => text.Split(',', StringSplitOptions.None).Select(x => int.Parse(x)).ToList());
    }
}