// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.CompilerServices;

namespace Snap.Hutao.Core.DependencyInjection;

/// <summary>
/// 服务提供器扩展
/// </summary>
internal static class ServiceProviderExtension
{
    /// <inheritdoc cref="ActivatorUtilities.CreateInstance{T}(IServiceProvider, object[])"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T CreateInstance<T>(this IServiceProvider serviceProvider, params object[] parameters)
    {
        return ActivatorUtilities.CreateInstance<T>(serviceProvider, parameters);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsDisposedSlow(this IServiceProvider? serviceProvider)
    {
        if (serviceProvider is null)
        {
            return false;
        }

        return serviceProvider.GetType().GetField("_disposed")?.GetValue(serviceProvider) is true;
    }
}