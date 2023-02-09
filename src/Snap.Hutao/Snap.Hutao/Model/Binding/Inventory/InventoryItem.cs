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
    /// <param name="saveCommand">保存命令</param>
    public InventoryItem(Material inner, Entity.InventoryItem entity, ICommand saveCommand)
    {
        Entity = entity;
        Inner = inner;
        count = entity.Count;
        SaveCountCommand = saveCommand;
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
    /// 保存个数命令
    /// </summary>
    public ICommand? SaveCountCommand { get; set; }

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
                SaveCountCommand?.Execute(this);
            }
        }
    }
}
