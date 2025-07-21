// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Model;
using Snap.Hutao.Model.Metadata.Item;

namespace Snap.Hutao.ViewModel.Cultivation;

internal sealed partial class CultivateItemView : ObservableObject, IEntityAccessWithMetadata<Model.Entity.CultivateItem, Material>
{
    private readonly TimeSpan offset;

    private CultivateItemView(Model.Entity.CultivateItem entity, Material inner, in TimeSpan offset)
    {
        Entity = entity;
        Inner = inner;
        this.offset = offset;
    }

    public Material Inner { get; }

    public Model.Entity.CultivateItem Entity { get; }

    public bool IsFinished
    {
        get => Entity.IsFinished;
        set => SetProperty(Entity.IsFinished, value, Entity, (entity, isFinished) => entity.IsFinished = isFinished);
    }

    public bool IsToday { get => Inner.IsItemOfToday(offset, true); }

    public DaysOfWeek DaysOfWeek { get => Inner.GetDaysOfWeek(); }

    public static CultivateItemView Create(Model.Entity.CultivateItem entity, Material inner, in TimeSpan offset)
    {
        return new(entity, inner, offset);
    }
}