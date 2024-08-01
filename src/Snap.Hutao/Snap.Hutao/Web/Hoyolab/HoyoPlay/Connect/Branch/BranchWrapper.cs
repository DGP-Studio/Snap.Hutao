// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.HoyoPlay.Connect.Branch;

internal sealed class BranchWrapper
{
    [JsonPropertyName("package_id")]
    public string PackageId { get; set; } = default!;

    [JsonPropertyName("branch")]
    public string Branch { get; set; } = default!;

    [JsonPropertyName("password")]
    public string Password { get; set; } = default!;

    [JsonPropertyName("tag")]
    public string Tag { get; set; } = default!;
}
