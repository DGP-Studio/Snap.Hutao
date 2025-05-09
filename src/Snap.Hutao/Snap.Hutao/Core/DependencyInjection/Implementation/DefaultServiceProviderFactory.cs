// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.DependencyInjection.Implementation;

/// <summary>
/// Default implementation of <see cref="IServiceProviderFactory{TContainerBuilder}"/>.
/// </summary>
public class DefaultServiceProviderFactory : IServiceProviderFactory<IServiceCollection>
{
    private readonly ServiceProviderOptions options;

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultServiceProviderFactory"/> class
    /// with default options.
    /// </summary>
    public DefaultServiceProviderFactory()
        : this(ServiceProviderOptions.Default)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultServiceProviderFactory"/> class
    /// with the specified <paramref name="options"/>.
    /// </summary>
    /// <param name="options">The options to use for this instance.</param>
    public DefaultServiceProviderFactory(ServiceProviderOptions options)
    {
        this.options = options ?? throw new ArgumentNullException(nameof(options));
    }

    /// <inheritdoc />
    public IServiceCollection CreateBuilder(IServiceCollection services)
    {
        return services;
    }

    /// <inheritdoc />
    public IServiceProvider CreateServiceProvider(IServiceCollection containerBuilder)
    {
        return containerBuilder.BuildServiceProvider(options);
    }
}