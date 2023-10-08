// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Request.Builder;
using System.Net.Http;
using System.Text;

namespace Snap.Hutao.Web.Hoyolab.DynamicSecret;

internal static class DynamicSecretHttpRequestMessageBuilderExtension
{
    private const string RandomRange = "abcdefghijklmnopqrstuvwxyz1234567890";

    public static async ValueTask SetDynamicSecretAsync(this HttpRequestMessageBuilder builder, DynamicSecretVersion version, SaltType saltType, bool includeChars)
    {
        string salt = HoyolabOptions.Salts[saltType];
        long t = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        string r = includeChars ? GetRandomStringWithChars() : GetRandomStringNoChars();

        string dsContent = $"salt={salt}&t={t}&r={r}";

        // ds2 b & q process
        if (version is DynamicSecretVersion.Gen2)
        {
            HttpContent? content = builder.Content;
            string b = content is not null
                ? await content.ReadAsStringAsync().ConfigureAwait(false)
                : saltType is SaltType.PROD ? "{}" : string.Empty; // PROD's default value is {}

            ArgumentNullException.ThrowIfNull(builder.RequestUri);
            string[] queries = Uri.UnescapeDataString(builder.RequestUri.Query).Split('?', 2); // queries[0] is always empty
            string q = queries.Length is 2 ? string.Join('&', queries[1].Split('&').OrderBy(x => x)) : string.Empty;

            dsContent = $"{dsContent}&b={b}&q={q}";
        }

#pragma warning disable CA1308
        string check = Core.Convert.ToMd5HexString(dsContent).ToLowerInvariant();
#pragma warning restore CA1308
        builder.SetHeader("DS", $"{t},{r},{check}");
    }

    private static string GetRandomStringWithChars()
    {
        StringBuilder sb = new(6);

        for (int i = 0; i < 6; i++)
        {
            int pos = Random.Shared.Next(0, RandomRange.Length);
            sb.Append(RandomRange[pos]);
        }

        return sb.ToString();
    }

    private static string GetRandomStringNoChars()
    {
        int rand = Random.Shared.Next(100000, 200000);
        return $"{(rand == 100000 ? 642367 : rand)}";
    }
}