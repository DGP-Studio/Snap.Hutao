// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Model;
using Snap.Hutao.Model.Metadata.Item;

namespace Snap.Hutao.ViewModel.Cultivation;

internal sealed partial class CultivateItemView : ObservableObject, IEntityAccessWithMetadata<Model.Entity.CultivateItem, Material>
{
    public CultivateItemView(Model.Entity.CultivateItem entity, Material inner)
    {
        Entity = entity;
        Inner = inner;
    }

    public Material Inner { get; }

    public Model.Entity.CultivateItem Entity { get; }

    public bool IsFinished
    {
        get => Entity.IsFinished;
        set => SetProperty(Entity.IsFinished, value, Entity, (entity, isFinished) => entity.IsFinished = isFinished);
    }

    public bool IsToday { get => Inner.IsTodaysItem(true); }

    public DaysOfWeek DaysOfWeek { get => Inner.GetDaysOfWeek(); }
}