// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Snap.Hutao.Core.Logging;

/// <summary>
/// Extension methods for the <see cref="ILoggerFactory"/> class.
/// </summary>
public static class DatabaseLoggerFactoryExtensions
{
    /// <summary>
    /// Adds a debug logger named 'Debug' to the factory.
    /// </summary>
    /// <param name="builder">The extension method argument.</param>
    /// <returns>日志构造器</returns>
    public static ILoggingBuilder AddDatabase(this ILoggingBuilder builder)
    {
        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, DatebaseLoggerProvider>());
        return builder;
    }
}