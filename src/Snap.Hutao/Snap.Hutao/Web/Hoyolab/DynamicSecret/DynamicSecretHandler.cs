// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Convert;
using Snap.Hutao.Web.Request;
using System.Collections.Immutable;
using System.Net.Http;
using System.Text;

namespace Snap.Hutao.Web.Hoyolab.DynamicSecret;

/// <summary>
/// 动态密钥处理器
/// </summary>
[Injection(InjectAs.Transient)]
public class DynamicSecretHandler : DelegatingHandler
{
    /// <summary>
    /// 盐
    /// </summary>
    // https://github.com/UIGF-org/Hoyolab.Salt
    public static readonly ImmutableDictionary<string, string> DynamicSecrets = new Dictionary<string, string>()
    {
        [nameof(SaltType.K2)] = "jrU9ULHGZdM9Os3uGHOpjyRELYxby5cg",
        [nameof(SaltType.LK2)] = "9gaxOdeeY2W9dw5x62pywhik8cxy5TIJ",
        [nameof(SaltType.X4)] = "xV8v4Qu54lUKrEYFZkJhB8cuOh9Asafs",
        [nameof(SaltType.X6)] = "t0qEgfub6cvueAPgR5m9aQWWVciEer7v",
        [nameof(SaltType.PROD)] = "JwYDpKvLj6MrMqqYU6jTKF17KNO2PXoS",
    }.ToImmutableDictionary();

    private const string RandomRange = "abcdefghijklmnopqrstuvwxyz1234567890";

    /// <inheritdoc/>
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken token)
    {
        if (request.Headers.TryGetValues("DS-Option", out IEnumerable<string>? values))
        {
            string[] definations = values.Single().Split('|');
            string version = definations[0];
            string saltType = definations[1];
            bool includeChars = definations[2] == "true";

            string salt = DynamicSecrets[saltType];

            long t = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            string r = includeChars ? GetRandomStringWithChars() : GetRandomStringNoChars();

            string dsContent = $"salt={salt}&t={t}&r={r}";

            if (version == nameof(DynamicSecretVersion.Gen2))
            {
                string b = request.Content != null
                    ? await request.Content.ReadAsStringAsync(token).ConfigureAwait(false)
                    : (saltType == nameof(SaltType.PROD) ? "{}" : string.Empty); // PROD's default value is {}

                string[] queries = Uri.UnescapeDataString(request.RequestUri!.Query).Split('?', 2);
                string q = queries.Length == 2 ? string.Join('&', queries[1].Split('&').OrderBy(x => x)) : string.Empty;

                dsContent = $"{dsContent}&b={b}&q={q}";
            }

            string check = Md5Convert.ToHexString(dsContent).ToLowerInvariant();

            request.Headers.Remove("DS-Option");
            request.Headers.Set("DS", $"{t},{r},{check}");
        }

        return await base.SendAsync(request, token).ConfigureAwait(false);
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