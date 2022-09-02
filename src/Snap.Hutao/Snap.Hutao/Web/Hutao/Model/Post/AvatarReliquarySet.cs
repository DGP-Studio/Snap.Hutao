// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hutao.Model.Post;

/// <summary>
/// 角色圣遗物套装
/// </summary>
public class AvatarReliquarySet
{
    /// <summary>
    /// 构造一个新的角色圣遗物套装
    /// </summary>
    /// <param name="id">套装Id</param>
    /// <param name="count">个数</param>
    public AvatarReliquarySet(int id, int count)
    {
        Id = id;
        Count = count;
    }

    /// <summary>
    /// 构造一个新的角色圣遗物套装
    /// </summary>
    /// <param name="kvp">键值对</param>
    public AvatarReliquarySet(KeyValuePair<int, int> kvp)
        : this(kvp.Key, kvp.Value)
    {
    }

    /// <summary>
    /// 套装Id
    /// </summary>
    public int Id { get; }

    /// <summary>
    /// 个数
    /// </summary>
    public int Count { get; }
}