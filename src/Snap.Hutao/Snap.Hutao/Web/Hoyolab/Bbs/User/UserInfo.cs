// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Bbs.User;

/// <summary>
/// 用户完整信息
/// </summary>
[HighQuality]
internal sealed class UserInfo
{
    /// <summary>
    /// 米游社Uid
    /// </summary>
    [JsonPropertyName("uid")]
    public string Uid { get; set; } = default!;

    /// <summary>
    /// 昵称
    /// </summary>
    [JsonPropertyName("nickname")]
    public string Nickname { get; set; } = default!;

    /// <summary>
    /// 简介
    /// </summary>
    [JsonPropertyName("introduce")]
    public string Introduce { get; set; } = default!;

    /// <summary>
    /// 头像Id
    /// </summary>
    [JsonPropertyName("avatar")]
    public string Avatar { get; set; } = default!;

    /// <summary>
    /// 性别
    /// </summary>
    [JsonPropertyName("gender")]
    public UserGender Gender { get; set; }

    /// <summary>
    /// 认证
    /// </summary>
    [JsonPropertyName("certification")]
    public Certification Certification { get; set; } = default!;

    /// <summary>
    /// 各个游戏的等级经验
    /// </summary>
    [JsonPropertyName("level_exps")]
    public List<LevelExperience> LevelExps { get; set; } = default!;

    /// <summary>
    /// 成就
    /// </summary>
    [JsonPropertyName("achieve")]
    public Achieve Achieve { get; set; } = default!;

    /// <summary>
    /// 社区信息
    /// </summary>
    [JsonPropertyName("community_info")]
    public CommunityInfo CommunityInfo { get; set; } = default!;

    /// <summary>
    /// 头像
    /// </summary>
    [JsonPropertyName("avatar_url")]
    public Uri AvatarUrl { get; set; } = default!;

    /// <summary>
    /// 认证详情
    /// </summary>
    [JsonPropertyName("certifications")]
    public List<DetailedCertification> Certifications { get; set; } = default!;

    /// <summary>
    /// 当前等级经验
    /// </summary>
    [JsonPropertyName("level_exp")]
    public LevelExperience LevelExp { get; set; } = default!;

    /// <summary>
    /// 头像框
    /// </summary>
    [JsonPropertyName("pendant")]
    public Uri? Pendant { get; set; }

    /// <summary>
    /// 是否登出
    /// </summary>
    [JsonPropertyName("is_logoff")]
    public bool IsLogOff { get; set; }

    /// <summary>
    /// IP地址
    /// </summary>
    [JsonPropertyName("ip_region")]
    public string IpRegion { get; set; } = default!;
}
