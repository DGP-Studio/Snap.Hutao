// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Request.Builder.Abstraction;
using System.Diagnostics;

namespace Snap.Hutao.Web.Request.Builder;

internal static class BuilderExtension
{
    [DebuggerStepThrough]
    public static T Configure<T>(this T builder, Action<T> configure)
        where T : IBuilder
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(configure);

        configure(builder);
        return builder;
    }

    [DebuggerStepThrough]
    public static T If<T>(this T builder, bool condition, Action<T> action)
        where T : IBuilder
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(action);

        if (condition)
        {
            action(builder);
        }

        return builder;
    }

    [DebuggerStepThrough]
    public static T IfNot<T>(this T builder, bool condition, Action<T> action)
        where T : IBuilder
    {
        return builder.If(!condition, action);
    }
}