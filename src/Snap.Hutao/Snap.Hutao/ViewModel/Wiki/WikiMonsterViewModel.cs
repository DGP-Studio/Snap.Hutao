// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Model.Metadata;
using Snap.Hutao.Model.Metadata.Item;
using Snap.Hutao.Model.Metadata.Monster;
using Snap.Hutao.Service.Metadata;
using Snap.Hutao.Service.Metadata.ContextAbstraction;
using Snap.Hutao.UI.Xaml.Data;
using System.Collections.Immutable;

namespace Snap.Hutao.ViewModel.Wiki;

[BindableCustomPropertyProvider]
[Service(ServiceLifetime.Scoped)]
internal sealed partial class WikiMonsterViewModel : Abstraction.ViewModel
{
    private readonly IMetadataService metadataService;
    private readonly ITaskContext taskContext;

    private WikiMonsterMetadataContext? metadataContext;

    [GeneratedConstructor]
    public partial WikiMonsterViewModel(IServiceProvider serviceProvider);

    public IAdvancedCollectionView<Monster>? Monsters
    {
        get;
        set
        {
            if (field is not null)
            {
                field.CurrentChanged -= OnCurrentMonsterChanged;
            }

            SetProperty(ref field, value);

            if (value is not null)
            {
                value.CurrentChanged += OnCurrentMonsterChanged;
            }
        }
    }

    [ObservableProperty]
    public partial BaseValueInfo? BaseValueInfo { get; set; }

    protected override async ValueTask<bool> LoadOverrideAsync(CancellationToken token)
    {
        if (await metadataService.InitializeAsync().ConfigureAwait(false))
        {
            try
            {
                metadataContext = await metadataService.GetContextAsync<WikiMonsterMetadataContext>(token).ConfigureAwait(false);

                foreach (Monster monster in metadataContext.Monsters)
                {
                    monster.DropsView ??= monster.Drops.EmptyIfDefault().SelectAsArray(static (i, context) => context.IdDisplayItemAndMaterialMap.GetValueOrDefault(i, Material.Default), metadataContext);
                }

                List<Monster> ordered = [.. metadataContext.Monsters.OrderBy(m => m.DescribeId.Value)];

                using (await EnterCriticalSectionAsync().ConfigureAwait(false))
                {
                    IAdvancedCollectionView<Monster> monstersView = ordered.AsAdvancedCollectionView();

                    await taskContext.SwitchToMainThreadAsync();
                    Monsters = monstersView;
                    Monsters.MoveCurrentToFirst();
                }

                return true;
            }
            catch (OperationCanceledException)
            {
            }
        }

        return false;
    }

    private void OnCurrentMonsterChanged(object? sender, object? e)
    {
        UpdateBaseValueInfo(Monsters?.CurrentItem);
    }

    private void UpdateBaseValueInfo(Monster? monster)
    {
        if (metadataContext is null || monster is not { GrowCurves: not null, BaseValue: not null })
        {
            BaseValueInfo = null;
            return;
        }

        BaseValueInfoMetadataContext context = new()
        {
            GrowCurveMap = metadataContext.LevelDictionaryMonsterGrowCurveMap,
            PromoteMap = default,
        };

        BaseValueInfo = new(Monster.MaxLevel, monster.GrowCurves.ToPropertyCurveValues(monster.BaseValue), context);
    }
}