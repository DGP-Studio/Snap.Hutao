// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hutao;

internal sealed class HutaoPackageInformation
{
    [JsonPropertyName("version")]
    public Version Version { get; set; } = default!;

    [JsonPropertyName("validation")]
    public string Validation { get; set; } = default!;

    [JsonPropertyName("mirrors")]
    public List<HutaoPackageMirror> Mirrors { get; set; } = default!;
}