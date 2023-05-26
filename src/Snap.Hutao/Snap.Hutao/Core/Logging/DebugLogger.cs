// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Diagnostics;

namespace Snap.Hutao.Core.Logging;

/// <summary>
/// A logger that writes messages in the debug output window only when a debugger is attached.
/// </summary>
internal sealed class DebugLogger : ILogger
{
    private readonly string name;

    /// <summary>
    /// Initializes a new instance of the <see cref="DebugLogger"/> class.
    /// </summary>
    /// <param name="name">The name of the logger.</param>
    public DebugLogger(string name)
    {
        this.name = name;
    }

    /// <inheritdoc />
    public IDisposable BeginScope<TState>(TState state)
        where TState : notnull
    {
        return NullScope.Instance;
    }

    /// <inheritdoc />
    public bool IsEnabled(LogLevel logLevel)
    {
        // If the filter is null, everything is enabled
        return logLevel != LogLevel.None;
    }

    /// <inheritdoc />
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
        {
            return;
        }

        ArgumentNullException.ThrowIfNull(formatter);

        string message = formatter(state, exception);

        if (string.IsNullOrEmpty(message))
        {
            return;
        }

        message = $"{logLevel}: {message}";

        if (exception != null)
        {
            message += Environment.NewLine + Environment.NewLine + exception;
        }

        DebugWriteLine(message, name);
    }

    private static void DebugWriteLine(string message, string name)
    {
        Debug.WriteLine(message, category: name);
    }
}