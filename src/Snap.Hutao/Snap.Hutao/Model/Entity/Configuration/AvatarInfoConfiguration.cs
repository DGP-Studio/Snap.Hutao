// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Snap.Hutao.Model.Entity.Configuration;

/// <summary>
/// 角色信息配置
/// </summary>
internal class AvatarInfoConfiguration : IEntityTypeConfiguration<AvatarInfo>
{
    /// <inheritdoc/>
    public void Configure(EntityTypeBuilder<AvatarInfo> builder)
    {
        builder.Property(e => e.Info)
            .HasColumnType("TEXT")
            .HasConversion<JsonTextValueConverter<Web.Enka.Model.AvatarInfo>>();
    }
}