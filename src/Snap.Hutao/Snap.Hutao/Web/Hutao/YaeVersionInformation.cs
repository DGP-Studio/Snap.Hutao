// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hutao;

internal sealed class YaeVersionInformation
{
    [JsonPropertyName("tagName")]
    public Version Version { get; set; } = default!;

    [JsonPropertyName("url")]
    public string Url { get; set; } = default!;

    [JsonPropertyName("source")]
    public string Source { get; set; } = default!;

    [JsonPropertyName("frameworkUrl")]
    public string FrameworkUrl { get; set; } = default!;
}