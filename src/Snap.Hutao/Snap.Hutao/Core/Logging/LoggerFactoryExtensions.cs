// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.Logging;

internal static class LoggerFactoryExtensions
{
    public static ILoggingBuilder AddConsoleWindow(this ILoggingBuilder builder)
    {
        builder.Services.AddSingleton<ConsoleWindowLifeTime>();

        builder.AddSimpleConsole();
        return builder;
    }
}