// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Snap.Hutao.Web.Hoyolab;

namespace Snap.Hutao.Model.Entity.Configuration;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    [SuppressMessage("", "SH007")]
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(e => e.CookieToken)
            .HasColumnType(SqliteTypeNames.Text)
            .HasConversion(e => e!.ToString(), e => Cookie.Parse(e));

        builder.Property(e => e.LToken)
            .HasColumnType(SqliteTypeNames.Text)
            .HasConversion(e => e!.ToString(), e => Cookie.Parse(e));

        builder.Property(e => e.SToken)
            .HasColumnType(SqliteTypeNames.Text)
            .HasConversion(e => e!.ToString(), e => Cookie.Parse(e));
    }
}