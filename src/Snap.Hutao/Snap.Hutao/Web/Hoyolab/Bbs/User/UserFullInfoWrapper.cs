// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Bbs.User;

/// <summary>
/// 用户完整信息包装器
/// </summary>
public class UserFullInfoWrapper
{
    /// <summary>
    /// 用户信息
    /// </summary>
    [JsonPropertyName("user_info")]
    public UserInfo UserInfo { get; set; } = default!;

    /// <summary>
    /// ?
    /// </summary>
    [JsonPropertyName("follow_relation")]
    public JsonElement? FollowRelation { get; set; }

    /// <summary>
    /// ?
    /// </summary>
    [JsonPropertyName("auth_relations")]
    public List<JsonElement> AuthRelations { get; set; } = default!;

    /// <summary>
    /// 是否被米游社封禁
    /// </summary>
    [JsonPropertyName("is_in_blacklist")]
    public bool IsInBlacklist { get; set; }

    /// <summary>
    /// ？
    /// </summary>
    [JsonPropertyName("is_has_collection")]
    public bool IsHasCollection { get; set; }

    /// <summary>
    /// 是否为创作者
    /// </summary>
    [JsonPropertyName("is_creator")]
    public bool IsCreator { get; set; }

    /// <summary>
    /// 售后服务
    /// </summary>
    [JsonPropertyName("customer_service")]
    public CustomerService CustomerService { get; set; } = default!;

    /// <summary>
    /// 审核信息
    /// </summary>
    [JsonPropertyName("audit_info")]
    public AuditInfo AuditInfo { get; set; } = default!;
}