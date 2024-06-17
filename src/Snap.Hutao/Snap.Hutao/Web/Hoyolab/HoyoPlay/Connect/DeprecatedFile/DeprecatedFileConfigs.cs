// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.HoyoPlay.Connect.DeprecatedFile;

internal sealed class DeprecatedFileConfigs
{
    [JsonPropertyName("deprecated_file_configs")]
    public List<DeprecatedFileConfig> Configs { get; set; } = default!;
}
