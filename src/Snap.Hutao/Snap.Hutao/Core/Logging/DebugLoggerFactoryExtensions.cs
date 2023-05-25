// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Snap.Hutao.Core.Logging;

/// <summary>
/// Extension methods for the <see cref="ILoggerFactory"/> class.
/// </summary>
internal static class DebugLoggerFactoryExtensions
{
    /// <summary>
    /// Adds a debug logger named 'Debug' to the factory.
    /// </summary>
    /// <param name="builder">The extension method argument.</param>
    /// <returns>builder</returns>
    public static ILoggingBuilder AddUnconditionalDebug(this ILoggingBuilder builder)
    {
        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, DebugLoggerProvider>());

        return builder;
    }
}