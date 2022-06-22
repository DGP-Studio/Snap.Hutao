// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore.Design;
using Snap.Hutao.Context.FileSystem;

namespace Snap.Hutao.Context.Database;

/// <summary>
/// 此类只用于在生成迁移时提供数据库上下文
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public class AppDbContextDesignTimeFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    /// <inheritdoc/>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public AppDbContext CreateDbContext(string[] args)
    {
        MyDocumentContext myDocument = new(new());
        myDocument.EnsureDirectory();

        string dbFile = myDocument.Locate("Userdata.db");
        string sqlConnectionString = $"Data Source={dbFile}";

        return AppDbContext.CreateFrom(sqlConnectionString);
    }
}
