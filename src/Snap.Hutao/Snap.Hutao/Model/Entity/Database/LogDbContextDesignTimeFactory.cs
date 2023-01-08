// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore.Design;
using Snap.Hutao.Model.Entity.Database;

namespace Snap.Hutao.Context.Database;

/// <summary>
/// 此类只用于在生成迁移时提供数据库上下文
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public class LogDbContextDesignTimeFactory : IDesignTimeDbContextFactory<LogDbContext>
{
    /// <inheritdoc/>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public LogDbContext CreateDbContext(string[] args)
    {
        string logDbName = System.IO.Path.Combine(Core.CoreEnvironment.DataFolder, "Log.db");
        return LogDbContext.Create($"Data Source={logDbName}");
    }
}