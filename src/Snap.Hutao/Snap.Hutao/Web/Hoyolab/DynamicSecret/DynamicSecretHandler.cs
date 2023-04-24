// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Request;
using System.Net.Http;

namespace Snap.Hutao.Web.Hoyolab.DynamicSecret;

/// <summary>
/// 动态密钥处理器
/// </summary>
[HighQuality]
[Injection(InjectAs.Transient)]
internal sealed class DynamicSecretHandler : DelegatingHandler
{
    /// <summary>
    /// 创建选项
    /// </summary>
    public const string OptionKeyName = "DS-Option";

    /// <inheritdoc/>
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken token)
    {
        if (request.Headers.TryGetValues(OptionKeyName, out IEnumerable<string>? values))
        {
            DynamicSecretCreationOptions options = new(values.Single());
            await ProcessRequestWithOptionsAsync(request, options, token).ConfigureAwait(false);
            request.Headers.Remove(OptionKeyName);
        }

        return await base.SendAsync(request, token).ConfigureAwait(false);
    }

    private static async Task ProcessRequestWithOptionsAsync(HttpRequestMessage request, DynamicSecretCreationOptions options, CancellationToken token)
    {
        string salt = Core.HoyolabOptions.Salts[options.SaltType];

        long t = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        string r = options.RandomString;

        string dsContent = $"salt={salt}&t={t}&r={r}";

        // ds2 b & q process
        if (options.Version == DynamicSecretVersion.Gen2)
        {
            string b = request.Content != null
                ? await request.Content.ReadAsStringAsync(token).ConfigureAwait(false)
                : options.DefaultBody; // PROD's default value is {}

            string[] queries = Uri.UnescapeDataString(request.RequestUri!.Query).Split('?', 2);
            string q = queries.Length == 2 ? string.Join('&', queries[1].Split('&').OrderBy(x => x)) : string.Empty;

            dsContent = $"{dsContent}&b={b}&q={q}";
        }

        string check = Core.Convert.ToMd5HexString(dsContent).ToLowerInvariant();
        request.Headers.Set("DS", $"{t},{r},{check}");
    }
}