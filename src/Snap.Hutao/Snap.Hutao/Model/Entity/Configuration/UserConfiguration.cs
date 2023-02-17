// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Snap.Hutao.Web.Hoyolab;

namespace Snap.Hutao.Model.Entity.Configuration;

/// <summary>
/// 用户配置
/// </summary>
[HighQuality]
internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    /// <inheritdoc/>
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(e => e.CookieToken)
            .HasColumnType("TEXT")
            .HasConversion(e => e!.ToString(), e => Cookie.Parse(e));

        builder.Property(e => e.Ltoken)
            .HasColumnType("TEXT")
            .HasConversion(e => e!.ToString(), e => Cookie.Parse(e));

        builder.Property(e => e.Stoken)
            .HasColumnType("TEXT")
            .HasConversion(e => e!.ToString(), e => Cookie.Parse(e));
    }
}