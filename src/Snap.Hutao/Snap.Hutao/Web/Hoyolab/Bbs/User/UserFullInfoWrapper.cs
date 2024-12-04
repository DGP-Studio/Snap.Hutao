// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Bbs.User;

internal sealed class UserFullInfoWrapper
{
    [JsonPropertyName("user_info")]
    public UserInfo UserInfo { get; set; } = default!;

    [JsonPropertyName("follow_relation")]
    public JsonElement? FollowRelation { get; set; }

    [JsonPropertyName("auth_relations")]
    public List<JsonElement> AuthRelations { get; set; } = default!;

    [JsonPropertyName("is_in_blacklist")]
    public bool IsInBlacklist { get; set; }

    [JsonPropertyName("is_has_collection")]
    public bool IsHasCollection { get; set; }

    [JsonPropertyName("is_creator")]
    public bool IsCreator { get; set; }

    [JsonPropertyName("customer_service")]
    public CustomerService CustomerService { get; set; } = default!;

    [JsonPropertyName("audit_info")]
    public AuditInfo AuditInfo { get; set; } = default!;
}