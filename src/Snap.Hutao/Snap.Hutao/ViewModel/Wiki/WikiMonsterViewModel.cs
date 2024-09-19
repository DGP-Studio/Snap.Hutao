// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Metadata.Item;
using Snap.Hutao.Model.Metadata.Monster;
using Snap.Hutao.Service.Metadata;
using Snap.Hutao.Service.Metadata.ContextAbstraction;
using Snap.Hutao.UI.Xaml.Data;
using System.Collections.Immutable;

namespace Snap.Hutao.ViewModel.Wiki;

[Injection(InjectAs.Scoped)]
[ConstructorGenerated]
internal sealed partial class WikiMonsterViewModel : Abstraction.ViewModel
{
    private readonly IMetadataService metadataService;
    private readonly ITaskContext taskContext;

    private AdvancedCollectionView<Monster>? monsters;
    private WikiMonsterMetadataContext metadataContext;
    private BaseValueInfo? baseValueInfo;

    public AdvancedCollectionView<Monster>? Monsters
    {
        get => monsters;
        set
        {
            if (monsters is not null)
            {
                monsters.CurrentChanged -= OnCurrentMonsterChanged;
            }

            SetProperty(ref monsters, value);

            if (value is not null)
            {
                value.CurrentChanged += OnCurrentMonsterChanged;
            }
        }
    }

    public BaseValueInfo? BaseValueInfo { get => baseValueInfo; set => SetProperty(ref baseValueInfo, value); }

    protected override async ValueTask<bool> InitializeOverrideAsync()
    {
        if (await metadataService.InitializeAsync().ConfigureAwait(false))
        {
            try
            {
                metadataContext = await metadataService.GetContextAsync<WikiMonsterMetadataContext>().ConfigureAwait(false);

                foreach (Monster monster in metadataContext.Monsters)
                {
                    monster.DropsView ??= monster.Drops?.SelectList(i => metadataContext.IdDisplayItemAndMaterialMap.GetValueOrDefault(i, Material.Default));
                }

                List<Monster> ordered = metadataContext.Monsters.OrderBy(m => m.RelationshipId.Value).ToList();

                using (await EnterCriticalSectionAsync().ConfigureAwait(false))
                {
                    AdvancedCollectionView<Monster> monstersView = ordered.ToAdvancedCollectionView();

                    await taskContext.SwitchToMainThreadAsync();
                    Monsters = monstersView;
                    Monsters.MoveCurrentToFirstOrDefault();
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
        if (monster is null || metadataContext is null)
        {
            BaseValueInfo = null;
            return;
        }

        BaseValueInfo = new(
            Monster.MaxLevel,
            monster.GrowCurves.SelectList(info => new PropertyCurveValue(info.Type, info.Value, monster.BaseValue.GetValue(info.Type))),
            metadataContext.LevelDictionaryMonsterGrowCurveMap);
    }
}