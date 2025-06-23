// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Immutable;

namespace Snap.Hutao.Web.Hoyolab.Bbs.User;

internal sealed class UserInfo
{
    [JsonPropertyName("uid")]
    public required string Uid { get; init; }

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

    [JsonPropertyName("level_exps")]
    public ImmutableArray<LevelExperience> LevelExps { get; init; }

    [JsonPropertyName("achieve")]
    public Achieve? Achieve { get; init; }

    [JsonPropertyName("community_info")]
    public CommunityInfo? CommunityInfo { get; init; }

    [JsonPropertyName("avatar_url")]
    public Uri? AvatarUrl { get; init; }

    [JsonPropertyName("certifications")]
    public ImmutableArray<DetailedCertification> Certifications { get; init; }

    [JsonPropertyName("level_exp")]
    public LevelExperience? LevelExp { get; init; }

    [JsonPropertyName("pendant")]
    public Uri? Pendant { get; init; }

    [JsonPropertyName("is_logoff")]
    public bool IsLogOff { get; init; }

    [JsonPropertyName("ip_region")]
    public string? IpRegion { get; init; }

    [JsonPropertyName("show_text")]
    public string? ShowText { get; init; }

    [JsonPropertyName("avatar_ext")]
    public AvatarExtend? AvatarExtend { get; init; }

    [JsonIgnore]
    public Uri AvatarUri
    {
        get
        {
            Uri? avatarUrl = AvatarExtend?.HdResources.SingleOrDefault(r => r.Format == AvatarExtend.AvatarType) is { Url: { } urlV2 }
                ? urlV2
                : AvatarUrl;

            string? source = avatarUrl?.OriginalString;
            if (avatarUrl is not null && !string.IsNullOrEmpty(source))
            {
                return avatarUrl;
            }

            string target = string.IsNullOrEmpty(IpRegion)
                    ? $"https://img-os-static.hoyolab.com/avatar/avatar{Avatar}.png"
                    : $"https://bbs-static.miyoushe.com/avatar/avatar{Avatar}.png";

            return target.ToUri();
        }
    }
}