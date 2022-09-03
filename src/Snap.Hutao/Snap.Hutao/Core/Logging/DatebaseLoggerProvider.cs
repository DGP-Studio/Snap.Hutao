// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.Logging;

/// <summary>
/// The provider for the <see cref="DatebaseLogger"/>.
/// </summary>
[ProviderAlias("Database")]
public sealed class DatebaseLoggerProvider : ILoggerProvider
{
    private readonly LogEntryQueue logEntryQueue = new();

    /// <inheritdoc/>
    public ILogger CreateLogger(string name)
    {
        return new DatebaseLogger(name, logEntryQueue);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        logEntryQueue.Dispose();
    }
}