// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Immutable;

namespace Snap.Hutao.Web.Hoyolab.Bbs.User;

internal sealed class AvatarExtend
{
    [JsonPropertyName("avatar_type")]
    public required int AvatarType { get; init; }

    [JsonPropertyName("avatar_assets_id")]
    public required string AvatarAssetsId { get; init; }

    [JsonPropertyName("resources")]
    public required ImmutableArray<AvatarExtendResource> Resources { get; init; }

    [JsonPropertyName("hd_resources")]
    public required ImmutableArray<AvatarExtendResource> HdResources { get; init; }
}