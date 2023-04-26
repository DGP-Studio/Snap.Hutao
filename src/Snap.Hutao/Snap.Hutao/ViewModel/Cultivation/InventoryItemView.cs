// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Model;
using Snap.Hutao.Model.Metadata.Item;

namespace Snap.Hutao.ViewModel.Cultivation;

/// <summary>
/// 背包物品
/// </summary>
[HighQuality]
internal sealed class InventoryItemView : ObservableObject, IEntityWithMetadata<Model.Entity.InventoryItem, Material>
{
    /// <summary>
    /// 创建一个新的背包物品
    /// </summary>
    /// <param name="entity">实体</param>
    /// <param name="inner">元数据</param>
    /// <param name="saveCommand">保存命令</param>
    public InventoryItemView(Model.Entity.InventoryItem entity, Material inner, ICommand saveCommand)
    {
        Entity = entity;
        Inner = inner;
        SaveCountCommand = saveCommand;
    }

    /// <summary>
    /// 实体
    /// </summary>
    public Model.Entity.InventoryItem Entity { get; set; }

    /// <summary>
    /// 元数据
    /// </summary>
    public Material Inner { get; set; }

    /// <summary>
    /// 保存个数命令
    /// </summary>
    public ICommand SaveCountCommand { get; set; }

    /// <summary>
    /// 个数
    /// </summary>
    public uint Count
    {
        get => Entity.Count; set
        {
            if (SetProperty(Entity.Count, value, Entity, (entity, count) => entity.Count = count))
            {
                SaveCountCommand.Execute(this);
            }
        }
    }
}
