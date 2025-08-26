// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Immutable;

namespace Snap.Hutao.Web.Hoyolab.Bbs.User;

internal class UserCommon
{
    [JsonPropertyName("uid")]
    public string? Uid { get; init; }

    [JsonPropertyName("nickname")]
    public string? Nickname { get; init; }

    [JsonPropertyName("introduce")]
    public string? Introduce { get; init; }

    [JsonPropertyName("avatar")]
    public string? Avatar { get; init; }

    [JsonPropertyName("gender")]
    public UserGender Gender { get; init; }

    [JsonPropertyName("certification")]
    public Certification? Certification { get; init; }

    [JsonPropertyName("level_exp")]
    public LevelExperience? LevelExp { get; init; }

    [JsonPropertyName("avatar_url")]
    public Uri? AvatarUrl { get; init; }

    [JsonPropertyName("pendant")]
    public Uri? Pendant { get; init; }

    [JsonPropertyName("certifications")]
    public ImmutableArray<DetailedCertification> Certifications { get; init; }
}