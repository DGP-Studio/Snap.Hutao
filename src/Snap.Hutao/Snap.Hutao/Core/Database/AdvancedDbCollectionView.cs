// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using Snap.Hutao.Core.Database.Abstraction;
using Snap.Hutao.Model;
using Snap.Hutao.Model.Entity.Database;
using Snap.Hutao.UI.Xaml.Data;

namespace Snap.Hutao.Core.Database;

internal sealed partial class AdvancedDbCollectionView<TEntity> : AdvancedCollectionView<TEntity>,
    IAdvancedDbCollectionView<TEntity>
    where TEntity : class, IPropertyValuesProvider, ISelectable
{
    private readonly IServiceProvider serviceProvider;

    private bool savingToDatabase = true;

    public AdvancedDbCollectionView(IList<TEntity> source, IServiceProvider serviceProvider)
        : base(source)
    {
        this.serviceProvider = serviceProvider;
    }

    public IDisposable SuppressChangeCurrentItem()
    {
        return new CurrentItemSuppression(this);
    }

    protected override void OnCurrentChangedOverride()
    {
        if (!savingToDatabase)
        {
            return;
        }

        TEntity? currentItem = CurrentItem;

        foreach (TEntity item in Source)
        {
            item.IsSelected = ReferenceEquals(item, currentItem);
        }

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            dbContext.Set<TEntity>().ExecuteUpdate(static update => update.SetProperty(entity => entity.IsSelected, false));

            if (currentItem is not null)
            {
                dbContext.Set<TEntity>().UpdateAndSave(currentItem);
            }
        }
    }

    private sealed partial class CurrentItemSuppression : IDisposable
    {
        private readonly AdvancedDbCollectionView<TEntity> view;
        private readonly TEntity? currentItem;

        public CurrentItemSuppression(AdvancedDbCollectionView<TEntity> view)
        {
            this.view = view;
            currentItem = view.CurrentItem;
            view.savingToDatabase = false;
        }

        public void Dispose()
        {
            view.MoveCurrentTo(currentItem);
            view.savingToDatabase = true;
        }
    }
}

[SuppressMessage("", "SA1402")]
internal sealed partial class AdvancedDbCollectionView<TEntityAccess, TEntity> : AdvancedCollectionView<TEntityAccess>,
    IAdvancedDbCollectionView<TEntityAccess>
    where TEntityAccess : class, IEntityAccess<TEntity>, IPropertyValuesProvider
    where TEntity : class, ISelectable
{
    private readonly IServiceProvider serviceProvider;

    private bool savingToDatabase = true;

    public AdvancedDbCollectionView(IList<TEntityAccess> source, IServiceProvider serviceProvider)
        : base(source)
    {
        this.serviceProvider = serviceProvider;
    }

    public IDisposable SuppressChangeCurrentItem()
    {
        return new CurrentItemSuppression(this);
    }

    protected override void OnCurrentChangedOverride()
    {
        if (!savingToDatabase)
        {
            return;
        }

        TEntityAccess? currentItem = CurrentItem;

        foreach (TEntityAccess item in Source)
        {
            item.Entity.IsSelected = ReferenceEquals(item, currentItem);
        }

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            dbContext.Set<TEntity>().ExecuteUpdate(static update => update.SetProperty(entity => entity.IsSelected, false));

            if (currentItem is not null)
            {
                dbContext.Set<TEntity>().UpdateAndSave(currentItem.Entity);
            }
        }
    }

    private sealed partial class CurrentItemSuppression : IDisposable
    {
        private readonly AdvancedDbCollectionView<TEntityAccess, TEntity> view;
        private readonly TEntityAccess? currentItem;

        public CurrentItemSuppression(AdvancedDbCollectionView<TEntityAccess, TEntity> view)
        {
            this.view = view;
            currentItem = view.CurrentItem;
            view.savingToDatabase = false;
        }

        public void Dispose()
        {
            view.MoveCurrentTo(currentItem);
            view.savingToDatabase = true;
        }
    }
}