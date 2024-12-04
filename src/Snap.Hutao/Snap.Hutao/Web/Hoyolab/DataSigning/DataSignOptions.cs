// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Request.Builder;
using System.Net.Http;

namespace Snap.Hutao.Web.Hoyolab.DataSigning;

internal sealed class DataSignOptions
{
    private DataSignOptions(SaltType type, bool includeChars, DataSignAlgorithmVersion version, string? body, string query)
    {
        Salt = HoyolabOptions.Salts[type];
        RandomString = includeChars ? Core.Random.GetLowerAndNumberString(6) : GetRandomNumberString();
        RequiresBodyAndQuery = version >= DataSignAlgorithmVersion.Gen2;
        string defaultBody = type is SaltType.PROD ? "{}" : string.Empty;
        Body = body ?? defaultBody;
        Query = query;
    }

    public string Salt { get; }

    public string RandomString { get; }

    public bool RequiresBodyAndQuery { get; }

    public string Body { get; }

    public string Query { get; }

    public static DataSignOptions CreateForGeneration1(SaltType type, bool includeChars)
    {
        return new(type, includeChars, DataSignAlgorithmVersion.Gen1, default, default!);
    }

    public static DataSignOptions CreateForGeneration2(SaltType type, bool includeChars, string? body, string query)
    {
        return new(type, includeChars, DataSignAlgorithmVersion.Gen2, body, query);
    }

    public static async ValueTask<DataSignOptions> CreateFromHttpRequestMessageBuilderAsync(HttpRequestMessageBuilder builder, SaltType type, bool includeChars, DataSignAlgorithmVersion version)
    {
        HttpContent? content = builder.Content;
        string? body = content is not null ? await content.ReadAsStringAsync().ConfigureAwait(false) : default;

        ArgumentNullException.ThrowIfNull(builder.RequestUri);
        string[] queries = Uri.UnescapeDataString(builder.RequestUri.Query).Split('?', 2); // queries[0] is always empty
        string query = queries.Length is 2 ? string.Join('&', queries[1].Split('&').OrderBy(x => x)) : string.Empty;

        return new(type, includeChars, version, body, query);
    }

    private static string GetRandomNumberString()
    {
        int rand = Random.Shared.Next(100000, 200000);
        return $"{(rand == 100000 ? 642367 : rand)}";
    }
}