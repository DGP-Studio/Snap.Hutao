// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Model;
using Snap.Hutao.Model.Metadata.Item;

namespace Snap.Hutao.ViewModel.Cultivation;

internal sealed partial class InventoryItemView : ObservableObject, IEntityAccessWithMetadata<Model.Entity.InventoryItem, Material>
{
    public InventoryItemView(Model.Entity.InventoryItem entity, Material inner, ICommand saveCommand)
    {
        Entity = entity;
        Inner = inner;
        SaveCountCommand = saveCommand;
    }

    public Model.Entity.InventoryItem Entity { get; }

    public Material Inner { get; }

    public ICommand SaveCountCommand { get; }

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
