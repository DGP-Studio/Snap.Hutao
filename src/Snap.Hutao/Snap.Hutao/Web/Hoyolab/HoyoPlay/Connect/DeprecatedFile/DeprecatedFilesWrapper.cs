// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.HoyoPlay.Connect.DeprecatedFile;

internal sealed class DeprecatedFilesWrapper : GameIndexedObject
{
    [JsonPropertyName("deprecated_files")]
    public List<DeprecatedFile> DeprecatedFiles { get; set; } = default!;
}
