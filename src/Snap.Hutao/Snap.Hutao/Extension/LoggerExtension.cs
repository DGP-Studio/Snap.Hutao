// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Extension;

/// <summary>
/// 日志器扩展
/// </summary>
[SuppressMessage("", "CA2254")]
public static class LoggerExtension
{
    /// <inheritdoc cref="LoggerExtensions.LogInformation(ILogger, string?, object?[])"/>
    public static T LogInformation<T>(this ILogger logger, string message, params object?[] param)
    {
        logger.LogInformation(message, param);
        return default!;
    }

    /// <inheritdoc cref="LoggerExtensions.LogWarning(ILogger, string?, object?[])"/>
    public static T LogWarning<T>(this ILogger logger, string message, params object?[] param)
    {
        logger.LogWarning(message, param);
        return default!;
    }
}