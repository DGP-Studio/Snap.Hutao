// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using Snap.Hutao.Context.Database;
using Snap.Hutao.Context.FileSystem;
using System.Diagnostics;
using System.Linq;

namespace Snap.Hutao.Core.Logging;

/// <summary>
/// The provider for the <see cref="DebugLogger"/>.
/// </summary>
[ProviderAlias("Database")]
public sealed class DatebaseLoggerProvider : ILoggerProvider
{
    private static readonly object LogDbContextLock = new();

    // the provider is created per logger, we don't want to create to much
    private static volatile LogDbContext? logDbContext;

    private static LogDbContext LogDbContext
    {
        get
        {
            if (logDbContext == null)
            {
                lock (LogDbContextLock)
                {
                    // prevent re-entry call
                    if (logDbContext == null)
                    {
                        MyDocumentContext myDocument = new(new());
                        logDbContext = LogDbContext.Create($"Data Source={myDocument.Locate("Log.db")}");
                        if (logDbContext.Database.GetPendingMigrations().Any())
                        {
                            Debug.WriteLine("[Debug] Performing LogDbContext Migrations");
                            logDbContext.Database.Migrate();
                        }

                        logDbContext.Logs.RemoveRange(logDbContext.Logs);
                        logDbContext.SaveChanges();
                    }
                }
            }

            return logDbContext;
        }
    }

    /// <inheritdoc/>
    public ILogger CreateLogger(string name)
    {
        return new DatebaseLogger(name, LogDbContext, LogDbContextLock);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        LogDbContext.Dispose();
    }
}
