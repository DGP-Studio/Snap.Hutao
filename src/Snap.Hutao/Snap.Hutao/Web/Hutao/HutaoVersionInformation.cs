// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hutao;

internal sealed class HutaoVersionInformation
{
    [JsonPropertyName("version")]
    public Version Version { get; set; } = default!;
}