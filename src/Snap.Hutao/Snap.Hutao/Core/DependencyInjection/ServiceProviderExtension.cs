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
    [Obsolete("Use ActivatorUtilities.CreateInstance<T>(IServiceProvider, params object[]) instead")]
    public static T CreateInstance<T>(this IServiceProvider serviceProvider, params object[] parameters)
    {
        return ActivatorUtilities.CreateInstance<T>(serviceProvider, parameters);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [Obsolete]
    public static bool IsDisposed(this IServiceProvider? serviceProvider, bool treatNullAsDisposed = true)
    {
        if (serviceProvider is null)
        {
            return treatNullAsDisposed;
        }

        if (serviceProvider is ServiceProvider serviceProviderImpl)
        {
            return GetPrivateDisposed(serviceProviderImpl);
        }

        return serviceProvider.GetType().GetField("_disposed")?.GetValue(serviceProvider) is true;
    }

    // private bool _disposed;
    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_disposed")]
    private static extern ref bool GetPrivateDisposed(ServiceProvider serviceProvider);
}