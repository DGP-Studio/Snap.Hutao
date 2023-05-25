// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.Logging;

/// <summary>
/// The provider for the <see cref="DebugLogger"/>.
/// </summary>
[ProviderAlias("Debug")]
internal sealed class DebugLoggerProvider : ILoggerProvider
{
    /// <inheritdoc />
    public ILogger CreateLogger(string name)
    {
        return new DebugLogger(name);
    }

    /// <inheritdoc />
    public void Dispose()
    {
    }
}