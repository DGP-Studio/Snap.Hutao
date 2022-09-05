// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using Snap.Hutao.Context.Database;
using Snap.Hutao.Context.FileSystem;
using Snap.Hutao.Extension;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace Snap.Hutao.Core.Logging;

/// <summary>
/// 日志队列
/// </summary>
public sealed class LogEntryQueue : IDisposable
{
    private readonly ConcurrentQueue<LogEntry> entryQueue = new();
    private readonly CancellationTokenSource disposeTokenSource = new();
    private readonly TaskCompletionSource writeDbCompletionSource = new();
    private readonly LogDbContext logDbContext;

    /// <summary>
    /// 构造一个新的日志队列
    /// </summary>
    public LogEntryQueue()
    {
        logDbContext = InitializeDbContext();

        Task.Run(async () => await WritePendingLogsAsync(disposeTokenSource.Token)).SafeForget();
    }

    /// <summary>
    /// 将日志消息存入队列
    /// </summary>
    /// <param name="logEntry">日志</param>
    public void Enqueue(LogEntry logEntry)
    {
        entryQueue.Enqueue(logEntry);
    }

    /// <inheritdoc/>
    [SuppressMessage("", "VSTHRD002")]
    public void Dispose()
    {
        // notify the write task to complete.
        disposeTokenSource.Cancel();

        // Wait the db operation complete.
        writeDbCompletionSource.Task.GetAwaiter().GetResult();

        logDbContext.Dispose();
    }

    private static LogDbContext InitializeDbContext()
    {
        HutaoContext myDocument = new(new());
        LogDbContext logDbContext = LogDbContext.Create($"Data Source={myDocument.Locate("Log.db")}");
        if (logDbContext.Database.GetPendingMigrations().Any())
        {
            Debug.WriteLine("[Debug] Performing LogDbContext Migrations");
            logDbContext.Database.Migrate();
        }

        logDbContext.Logs.RemoveRange(logDbContext.Logs);
        logDbContext.SaveChanges();

        return logDbContext;
    }

    private async Task WritePendingLogsAsync(CancellationToken token)
    {
        bool hasAdded = false;
        while (true)
        {
            if (entryQueue.TryDequeue(out LogEntry? logEntry))
            {
                logDbContext.Logs.Add(logEntry);
                hasAdded = true;
            }
            else
            {
                if (hasAdded)
                {
                    logDbContext.SaveChanges();
                    hasAdded = false;
                }

                if (token.IsCancellationRequested)
                {
                    writeDbCompletionSource.TrySetResult();
                    break;
                }

                try
                {
                    await Task.Delay(5000, token).ConfigureAwait(false);
                }
                catch (TaskCanceledException)
                {
                }
            }
        }
    }
}