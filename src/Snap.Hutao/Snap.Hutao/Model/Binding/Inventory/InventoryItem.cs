// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Model.Metadata;

namespace Snap.Hutao.Model.Binding.Inventory;

/// <summary>
/// 背包物品
/// </summary>
internal class InventoryItem : ObservableObject
{
    private uint count;

    /// <summary>
    /// 创建一个新的背包物品
    /// </summary>
    /// <param name="inner">元数据</param>
    /// <param name="entity">实体</param>
    public InventoryItem(Material inner, Entity.InventoryItem entity)
    {
        Entity = entity;
        Inner = inner;
        count = entity.Count;
    }

    /// <summary>
    /// 实体
    /// </summary>
    public Entity.InventoryItem Entity { get; set; }

    /// <summary>
    /// 元数据
    /// </summary>
    public Material Inner { get; set; }

    /// <summary>
    /// 个数
    /// </summary>
    public uint Count
    {
        get => count; set
        {
            if (SetProperty(ref count, value))
            {
                Entity.Count = value;
                Ioc.Default.GetRequiredService<Service.Cultivation.ICultivationService>().SaveInventoryItem(this);
            }
        }
    }
}
