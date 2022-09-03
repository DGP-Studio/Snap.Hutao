// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using Snap.Hutao.Context.Database;
using Snap.Hutao.Context.FileSystem;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;

namespace Snap.Hutao.Core.Logging;

/// <summary>
/// 日志队列
/// </summary>
public sealed class LogEntryQueue : IDisposable
{
    private static readonly object LogDbContextCreationLock = new();

    private readonly ConcurrentQueue<LogEntry> entryQueue = new();
    private readonly CancellationTokenSource disposeCancellationTokenSource = new();
    private readonly TaskCompletionSource writeDbTaskCompletionSource = new();

    // the provider is created per logger, we don't want to create too much
    private volatile LogDbContext? logDbContext;

    /// <summary>
    /// 构造一个新的日志队列
    /// </summary>
    public LogEntryQueue()
    {
        Execute();
    }

    private LogDbContext LogDbContext
    {
        get
        {
            if (logDbContext == null)
            {
                lock (LogDbContextCreationLock)
                {
                    // prevent re-entry call
                    if (logDbContext == null)
                    {
                        HutaoContext myDocument = new(new());
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
        disposeCancellationTokenSource.Cancel();
        writeDbTaskCompletionSource.Task.GetAwaiter().GetResult();

        LogDbContext.Dispose();
    }

    [SuppressMessage("", "VSTHRD100")]
    private async void Execute()
    {
        await Task.Run(async () => await ExecuteCoreAsync(disposeCancellationTokenSource.Token));
    }

    private async Task ExecuteCoreAsync(CancellationToken token)
    {
        bool hasAdded = false;
        while (true)
        {
            if (entryQueue.TryDequeue(out LogEntry? logEntry))
            {
                LogDbContext.Logs.Add(logEntry);
                hasAdded = true;
            }
            else
            {
                if (hasAdded)
                {
                    LogDbContext.SaveChanges();
                    hasAdded = false;
                }

                if (token.IsCancellationRequested)
                {
                    writeDbTaskCompletionSource.TrySetResult();
                    break;
                }

                await Task
                    .Delay(1000, CancellationToken.None)
                    .ConfigureAwait(false);
            }
        }
    }
}