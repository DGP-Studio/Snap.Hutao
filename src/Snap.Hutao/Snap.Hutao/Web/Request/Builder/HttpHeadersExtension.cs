// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Net.Http.Headers;

namespace Snap.Hutao.Web.Request.Builder;

internal static class HttpHeadersExtension
{
    public static void AddWithUnknownValueCount(this HttpHeaders headers, string name, IEnumerable<string?>? values)
    {
        ArgumentNullException.ThrowIfNull(name);

        // We have to work around the .NET API a little bit. See the comment below for details.
        values ??= Enumerable.Empty<string?>();
        values = values.Where(v => v is not null);

        if (values.Any())
        {
            headers.Add(name, values);
        }
        else
        {
            // According to https://docs.microsoft.com/en-us/dotnet/api/system.net.http.headers.httpheaders.add?f1url=https%3A%2F%2Fmsdn.microsoft.com%2Fquery%2Fdev15.query%3FappId%3DDev15IDEF1%26l%3DEN-US%26k%3Dk(System.Net.Http.Headers.HttpHeaders.Add)%3Bk(SolutionItemsProject)%3Bk(DevLang-csharp)%26rd%3Dtrue&view=netframework-4.8#System_Net_Http_Headers_HttpHeaders_Add_System_String_System_Collections_Generic_IEnumerable_System_String__
            // the HttpRequestMessage doesn't accept null/empty enumerables.
            // The Add(string, string) method does though.
            // -> We can use this one to add a header with an empty value.
            headers.Add(name, string.Empty);
        }
    }

    public static void Remove(this HttpHeaders headers, IEnumerable<string?>? names)
    {
        if (names is null)
        {
            return;
        }

        foreach (string? name in names)
        {
            if (name is not null)
            {
                headers.Remove(name);
            }
        }
    }
}