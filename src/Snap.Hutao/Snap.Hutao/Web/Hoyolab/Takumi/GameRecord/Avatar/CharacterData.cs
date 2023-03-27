// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.Avatar;

/// <summary>
/// 角色 POST 数据
/// </summary>
internal sealed class CharacterData
{
    /// <summary>
    /// 构造一个新的角色 POST 数据
    /// </summary>
    /// <param name="uid">uid</param>
    /// <param name="characterIds">角色id</param>
    public CharacterData(PlayerUid uid, IEnumerable<AvatarId> characterIds)
    {
        CharacterIds = characterIds;
        Uid = uid.Value;
        Server = uid.Region;
    }

    /// <summary>
    /// 角色id
    /// </summary>
    [JsonPropertyName("character_ids")]
    public IEnumerable<AvatarId> CharacterIds { get; }

    /// <summary>
    /// uid
    /// </summary>
    [JsonPropertyName("role_id")]
    public string Uid { get; } = default!;

    /// <summary>
    /// 服务器
    /// </summary>
    [JsonPropertyName("server")]
    public string Server { get; }
}