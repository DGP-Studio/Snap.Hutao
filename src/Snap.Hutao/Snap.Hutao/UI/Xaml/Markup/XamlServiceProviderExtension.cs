// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Markup;

namespace Snap.Hutao.UI.Xaml.Markup;

/// <summary>
/// Xaml 服务提供器扩展
/// </summary>
internal static class XamlServiceProviderExtension
{
    /// <summary>
    /// Get IProvideValueTarget from serviceProvider
    /// </summary>
    /// <param name="provider">serviceProvider</param>
    /// <returns>IProvideValueTarget</returns>
    public static IProvideValueTarget GetProvideValueTarget(this IXamlServiceProvider provider)
    {
        return (IProvideValueTarget)provider.GetService(typeof(IProvideValueTarget));
    }

    /// <summary>
    /// Get IRootObjectProvider from serviceProvider
    /// </summary>
    /// <param name="provider">serviceProvider</param>
    /// <returns>IRootObjectProvider</returns>
    public static IRootObjectProvider GetRootObjectProvider(this IXamlServiceProvider provider)
    {
        return (IRootObjectProvider)provider.GetService(typeof(IRootObjectProvider));
    }

    /// <summary>
    /// Get IUriContext from serviceProvider
    /// </summary>
    /// <param name="provider">serviceProvider</param>
    /// <returns>IUriContext</returns>
    public static IUriContext GetUriContext(this IXamlServiceProvider provider)
    {
        return (IUriContext)provider.GetService(typeof(IUriContext));
    }

    /// <summary>
    /// Get IXamlTypeResolver from serviceProvider
    /// </summary>
    /// <param name="provider">serviceProvider</param>
    /// <returns>IXamlTypeResolver</returns>
    public static IXamlTypeResolver GetXamlTypeResolver(this IXamlServiceProvider provider)
    {
        return (IXamlTypeResolver)provider.GetService(typeof(IXamlTypeResolver));
    }
}