// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Context.Database;

namespace Snap.Hutao.Core.Logging;

/// <summary>
/// A logger that writes messages in the database table
/// </summary>
internal sealed partial class DatebaseLogger : ILogger
{
    private readonly string name;
    private readonly LogDbContext logDbContext;
    private readonly object logDbContextLock;

    /// <summary>
    /// Initializes a new instance of the <see cref="DatebaseLogger"/> class.
    /// </summary>
    /// <param name="name">The name of the logger.</param>
    /// <param name="logDbContext">应用程序数据库上下文</param>
    /// <param name="logDbContextLock">上下文锁</param>
    public DatebaseLogger(string name, LogDbContext logDbContext, object logDbContextLock)
    {
        this.name = name;
        this.logDbContext = logDbContext;
        this.logDbContextLock = logDbContextLock;
    }

    /// <inheritdoc />
    public IDisposable BeginScope<TState>(TState state)
    {
        return new NullScope();
    }

    /// <inheritdoc />
    public bool IsEnabled(LogLevel logLevel)
    {
        return logLevel != LogLevel.None;
    }

    /// <inheritdoc />
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, System.Exception? exception, Func<TState, System.Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
        {
            return;
        }

        string message = Must.NotNull(formatter)(state, exception);

        if (string.IsNullOrEmpty(message))
        {
            return;
        }

        LogEntry entry = new()
        {
            Category = name,
            LogLevel = logLevel,
            EventId = eventId.Id,
            Message = message,
            Exception = exception?.ToString(),
        };

        // DbContext is not a thread safe class, so we have to lock the wirte procedure
        lock (logDbContextLock)
        {
            logDbContext.Logs.Add(entry);
            logDbContext.SaveChanges();
        }
    }

    /// <summary>
    /// An empty scope without any logic
    /// </summary>
    private class NullScope : IDisposable
    {
        public NullScope()
        {
        }

        /// <inheritdoc />
        public void Dispose()
        {
        }
    }
}