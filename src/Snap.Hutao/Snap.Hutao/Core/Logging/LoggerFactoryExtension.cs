// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.Logging;

internal static class LoggerFactoryExtension
{
    public static ILoggingBuilder AddConsoleWindow(this ILoggingBuilder builder)
    {
        builder.Services.AddSingleton<ConsoleWindowLifeTime>();

        builder.AddSimpleConsole(options =>
        {
            options.TimestampFormat = "yyyy-MM-dd HH:mm:ss.fff ";
        });

        return builder;
    }
}