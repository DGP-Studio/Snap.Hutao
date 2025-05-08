// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.DependencyInjection.Implementation;

/// <summary>
/// Extension methods for building a <see cref="ServiceProvider"/> from an <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionContainerBuilderExtensions
{
    /// <summary>
    /// Creates a <see cref="ServiceProvider"/> containing services from the provided <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> containing service descriptors.</param>
    /// <returns>The <see cref="ServiceProvider"/>.</returns>
    public static ServiceProvider BuildServiceProvider(this IServiceCollection services)
    {
        return BuildServiceProvider(services, ServiceProviderOptions.Default);
    }

    /// <summary>
    /// Creates a <see cref="ServiceProvider"/> containing services from the provided <see cref="IServiceCollection"/>
    /// optionally enabling scope validation.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> containing service descriptors.</param>
    /// <param name="validateScopes">
    /// <c>true</c> to perform check verifying that scoped services never gets resolved from root provider; otherwise <c>false</c>.
    /// </param>
    /// <returns>The <see cref="ServiceProvider"/>.</returns>
    public static ServiceProvider BuildServiceProvider(this IServiceCollection services, bool validateScopes)
    {
        return services.BuildServiceProvider(new ServiceProviderOptions
        {
            ValidateScopes = validateScopes,
        });
    }

    public static ServiceProvider BuildServiceProvider(this IServiceCollection services, bool validateScopes, bool validateOnBuild)
    {
        return services.BuildServiceProvider(new ServiceProviderOptions
        {
            ValidateScopes = validateScopes,
            ValidateOnBuild = validateOnBuild,
        });
    }

    /// <summary>
    /// Creates a <see cref="ServiceProvider"/> containing services from the provided <see cref="IServiceCollection"/>
    /// optionally enabling scope validation.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> containing service descriptors.</param>
    /// <param name="options">
    /// Configures various service provider behaviors.
    /// </param>
    /// <returns>The <see cref="ServiceProvider"/>.</returns>
    public static ServiceProvider BuildServiceProvider(this IServiceCollection services, ServiceProviderOptions options)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(options);

        return new(services, options);
    }
}