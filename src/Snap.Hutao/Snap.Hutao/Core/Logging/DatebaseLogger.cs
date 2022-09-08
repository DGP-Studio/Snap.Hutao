// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.Logging;

/// <summary>
/// A logger that writes messages in the database table
/// </summary>
internal sealed partial class DatebaseLogger : ILogger
{
    private readonly string name;
    private readonly LogEntryQueue logEntryQueue;

    /// <summary>
    /// Initializes a new instance of the <see cref="DatebaseLogger"/> class.
    /// </summary>
    /// <param name="name">The name of the logger.</param>
    /// <param name="logEntryQueue">日志队列</param>
    public DatebaseLogger(string name, LogEntryQueue logEntryQueue)
    {
        this.name = name;
        this.logEntryQueue = logEntryQueue;
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

        string message = formatter(state, exception);

        if (string.IsNullOrEmpty(message))
        {
            return;
        }

        LogEntry entry = new()
        {
            Time = DateTimeOffset.Now,
            Category = name,
            LogLevel = logLevel,
            EventId = eventId.Id,
            Message = message,
            Exception = exception?.ToString(),
        };

        logEntryQueue.Enqueue(entry);
    }

    /// <summary>
    /// An empty scope without any logic
    /// </summary>
    private struct NullScope : IDisposable
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