// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Specialized;
using System.Text;
using System.Web;

namespace Snap.Hutao.Web.Request;

internal static class NameValueCollectionExtension
{
    private static readonly Type HttpQSCollectionType = HttpUtility.ParseQueryString(string.Empty).GetType();

    public static string ToQueryString(this NameValueCollection collection)
    {
        if (collection.GetType() == HttpQSCollectionType)
        {
#pragma warning disable SH007
            return collection.ToString()!;
#pragma warning restore SH007
        }

        int count = collection.Count;
        if (count == 0)
        {
            return string.Empty;
        }

        StringBuilder sb = new();
        foreach (string? key in collection.AllKeys)
        {
            if (collection.GetValues(key) is not { } values)
            {
                continue;
            }

            foreach (ref readonly string value in values.AsSpan())
            {
                if (!string.IsNullOrEmpty(key))
                {
                    sb.Append(key).Append('=');
                }

                sb.Append(HttpUtility.UrlEncode(value)).Append('&');
            }
        }

        return sb.Length > 0 ? sb.ToString(0, sb.Length - 1) : string.Empty;
    }
}