// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.Logging;

[SuppressMessage("", "SA1611")]
[SuppressMessage("", "SA1615")]
internal static class BreadcrumbFactory
{
    /// <summary>
    /// If the category is 'console' then will display as 'debug'<br/>
    /// If the category is 'navigation' then will display as 'navigation'<br/>
    /// If the category is 'sentry.transaction' then will display as 'transaction'<br/>
    /// If the category is 'sentry.event' then will display as 'transaction'<br/>
    /// If the category is 'ui.*' then will display as 'ui'<br/>
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static Breadcrumb CreateDefault(string message, string? category = null, IReadOnlyDictionary<string, string>? data = null, BreadcrumbLevel level = default)
    {
        return new(message, "default", data, category, level);
    }

    public static Breadcrumb CreateDebug(string message, string? category = null, IReadOnlyDictionary<string, string>? data = null, BreadcrumbLevel level = default)
    {
        return new(message, "debug", data, category, level);
    }

    public static Breadcrumb CreateError(string message, string? category = null, IReadOnlyDictionary<string, string>? data = null, BreadcrumbLevel level = default)
    {
        return new(message, "error", data, category, level);
    }

    public static Breadcrumb CreateNavigation(string from, string to, string? category = null, IReadOnlyDictionary<string, string>? data = null, BreadcrumbLevel level = default)
    {
        Dictionary<string, string> data2 = new(data ?? new Dictionary<string, string>())
        {
            ["from"] = from,
            ["to"] = to,
        };
        return new(default!, "navigation", data2, category, level);
    }

    public static Breadcrumb CreateInfo(string message, string? category = null, IReadOnlyDictionary<string, string>? data = null, BreadcrumbLevel level = default)
    {
        return new(message, "info", data, category, level);
    }

    public static Breadcrumb CreateQuery(string query, string? category = null, IReadOnlyDictionary<string, string>? data = null, BreadcrumbLevel level = default)
    {
        return new(query, "query", data, category, level);
    }

    public static Breadcrumb CreateTransaction(string transaction, string? category = null, IReadOnlyDictionary<string, string>? data = null, BreadcrumbLevel level = default)
    {
        return new(transaction, "transaction", data, category, level);
    }

    public static Breadcrumb CreateUI(string message, string? category = null, IReadOnlyDictionary<string, string>? data = null, BreadcrumbLevel level = default)
    {
        return new(message, "ui", data, category, level);
    }

    public static Breadcrumb CreateUser(string message, string? category = null, IReadOnlyDictionary<string, string>? data = null, BreadcrumbLevel level = default)
    {
        return new(message, "user", data, category, level);
    }
}