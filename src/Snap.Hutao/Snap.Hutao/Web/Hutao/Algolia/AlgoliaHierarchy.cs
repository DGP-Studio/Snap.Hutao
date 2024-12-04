// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Web;

namespace Snap.Hutao.Web.Hutao.Algolia;

internal sealed class AlgoliaHierarchy
{
    [JsonPropertyName("lvl1")]
    public string? Lvl1 { get; set; }

    [JsonPropertyName("lvl2")]
    public string? Lvl2 { get; set; }

    [JsonPropertyName("lvl3")]
    public string? Lvl3 { get; set; }

    [JsonPropertyName("lvl4")]
    public string? Lvl4 { get; set; }

    [JsonPropertyName("lvl5")]
    public string? Lvl5 { get; set; }

    [JsonPropertyName("lvl6")]
    public string? Lvl6 { get; set; }

    public IEnumerable<string> DisplayLevels
    {
        get
        {
            return GetDisplayLevels();
            IEnumerable<string> GetDisplayLevels()
            {
                if (string.IsNullOrEmpty(Lvl1))
                {
                    yield break;
                }

                yield return HttpUtility.HtmlDecode(Lvl1);

                if (string.IsNullOrEmpty(Lvl2))
                {
                    yield break;
                }

                yield return HttpUtility.HtmlDecode(Lvl2);

                if (string.IsNullOrEmpty(Lvl3))
                {
                    yield break;
                }

                yield return HttpUtility.HtmlDecode(Lvl3);

                if (string.IsNullOrEmpty(Lvl4))
                {
                    yield break;
                }

                yield return HttpUtility.HtmlDecode(Lvl4);

                if (string.IsNullOrEmpty(Lvl5))
                {
                    yield break;
                }

                yield return HttpUtility.HtmlDecode(Lvl5);

                if (string.IsNullOrEmpty(Lvl6))
                {
                    yield break;
                }

                yield return HttpUtility.HtmlDecode(Lvl6);
            }
        }
    }
}