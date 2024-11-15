// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Bbs.User;

internal sealed class UserInfo
{
    [JsonPropertyName("uid")]
    public string Uid { get; set; } = default!;

    [JsonPropertyName("nickname")]
    public string Nickname { get; set; } = default!;

    [JsonPropertyName("introduce")]
    public string Introduce { get; set; } = default!;

    [JsonPropertyName("avatar")]
    public string Avatar { get; set; } = default!;

    [JsonPropertyName("gender")]
    public UserGender Gender { get; set; }

    [JsonPropertyName("certification")]
    public Certification Certification { get; set; } = default!;

    [JsonPropertyName("level_exps")]
    public List<LevelExperience> LevelExps { get; set; } = default!;

    [JsonPropertyName("achieve")]
    public Achieve Achieve { get; set; } = default!;

    [JsonPropertyName("community_info")]
    public CommunityInfo CommunityInfo { get; set; } = default!;

    [JsonPropertyName("avatar_url")]
    public Uri AvatarUrl { get; set; } = default!;

    [JsonPropertyName("certifications")]
    public List<DetailedCertification> Certifications { get; set; } = default!;

    [JsonPropertyName("level_exp")]
    public LevelExperience LevelExp { get; set; } = default!;

    [JsonPropertyName("pendant")]
    public Uri? Pendant { get; set; }

    [JsonPropertyName("is_logoff")]
    public bool IsLogOff { get; set; }

    [JsonPropertyName("ip_region")]
    public string IpRegion { get; set; } = default!;

    [JsonIgnore]
    public Uri AvatarUri
    {
        get
        {
            string? source = AvatarUrl?.OriginalString;
            if (!string.IsNullOrEmpty(source))
            {
                ArgumentNullException.ThrowIfNull(AvatarUrl);
                return AvatarUrl;
            }

            string target = string.IsNullOrEmpty(IpRegion)
                    ? $"https://img-os-static.hoyolab.com/avatar/avatar{Avatar}.png"
                    : $"https://bbs-static.miyoushe.com/avatar/avatar{Avatar}.png";

            return target.ToUri();
        }
    }
}
