// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.DependencyInjection.Implementation;

/// <summary>
/// Options for configuring various behaviors of the default <see cref="IServiceProvider"/> implementation.
/// </summary>
public class ServiceProviderOptions
{
    // Avoid allocating objects in the default case
    internal static readonly ServiceProviderOptions Default = new();

    /// <summary>
    /// Gets or sets a value that indicates whether validation is performed to ensure that scoped services are never resolved from the root provider.
    /// </summary>
    public bool ValidateScopes { get; set; }

    /// <summary>
    /// Gets or sets a value that indicates whether validation is performed to ensure all services can be created when <see cref="ServiceCollectionContainerBuilderExtensions.BuildServiceProvider(IServiceCollection, ServiceProviderOptions)" /> is called.
    /// </summary>
    /// <remarks>
    /// Open generics services aren't validated.
    /// </remarks>
    public bool ValidateOnBuild { get; set; }
}