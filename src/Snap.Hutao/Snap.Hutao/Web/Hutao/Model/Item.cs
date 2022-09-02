// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hutao.Model;

/// <summary>
/// 胡桃数据库物品
/// </summary>
public class Item
{
    /// <summary>
    /// 构造一个新的胡桃数据库物品
    /// </summary>
    /// <param name="id">物品Id</param>
    /// <param name="name">名称</param>
    /// <param name="url">链接</param>
    [JsonConstructor]
    public Item(int id, string name, string url)
    {
        Id = id;
        Name = name;
        Url = url;
    }

    /// <summary>
    /// 物品Id
    /// </summary>
    public int Id { get; }

    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// 链接
    /// </summary>
    public string Url { get; }
}