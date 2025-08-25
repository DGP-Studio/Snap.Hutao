// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.Logging;

internal static class BreadcrumbFactory2
{
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static Breadcrumb CreateDefault(string message, string? category = null, (string, string)[]? data = null, BreadcrumbLevel level = default)
    {
        return BreadcrumbFactory.CreateDebug(message, category, data?.ToDictionary(), level);
    }

    public static Breadcrumb CreateDebug(string message, string? category = null, (string, string)[]? data = null, BreadcrumbLevel level = default)
    {
        return BreadcrumbFactory.CreateDebug(message, category, data?.ToDictionary(), level);
    }

    public static Breadcrumb CreateError(string message, string? category = null, (string, string)[]? data = null, BreadcrumbLevel level = default)
    {
        return BreadcrumbFactory.CreateError(message, category, data?.ToDictionary(), level);
    }

    public static Breadcrumb CreateNavigation(string from, string to, string? category = null, (string, string)[]? data = null, BreadcrumbLevel level = default)
    {
        return BreadcrumbFactory.CreateNavigation(from, to, category, data?.ToDictionary(), level);
    }

    public static Breadcrumb CreateInfo(string message, string? category = null, (string, string)[]? data = null, BreadcrumbLevel level = default)
    {
        return BreadcrumbFactory.CreateInfo(message, category, data?.ToDictionary(), level);
    }

    public static Breadcrumb CreateQuery(string query, string? category = null, (string, string)[]? data = null, BreadcrumbLevel level = default)
    {
        return BreadcrumbFactory.CreateQuery(query, category, data?.ToDictionary(), level);
    }

    public static Breadcrumb CreateTransaction(string transaction, string? category = null, (string, string)[]? data = null, BreadcrumbLevel level = default)
    {
        return BreadcrumbFactory.CreateTransaction(transaction, category, data?.ToDictionary(), level);
    }

    public static Breadcrumb CreateUI(string message, string? category = null, (string, string)[]? data = null, BreadcrumbLevel level = default)
    {
        return BreadcrumbFactory.CreateUI(message, category, data?.ToDictionary(), level);
    }

    public static Breadcrumb CreateUser(string message, string? category = null, (string, string)[]? data = null, BreadcrumbLevel level = default)
    {
        return BreadcrumbFactory.CreateUser(message, category, data?.ToDictionary(), level);
    }
}