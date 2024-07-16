// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Metadata.Item;
using Snap.Hutao.Model.Metadata.Monster;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.Service.Metadata;
using Snap.Hutao.UI.Xaml.Data;

namespace Snap.Hutao.ViewModel.Wiki;

/// <summary>
/// 怪物资料视图模型
/// </summary>
[Injection(InjectAs.Scoped)]
[ConstructorGenerated]
internal sealed partial class WikiMonsterViewModel : Abstraction.ViewModel
{
    private readonly IMetadataService metadataService;
    private readonly ITaskContext taskContext;

    private AdvancedCollectionView<Monster>? monsters;
    private Monster? selected;
    private BaseValueInfo? baseValueInfo;
    private Dictionary<Level, Dictionary<GrowCurveType, float>>? levelMonsterCurveMap;

    /// <summary>
    /// 角色列表
    /// </summary>
    public AdvancedCollectionView<Monster>? Monsters { get => monsters; set => SetProperty(ref monsters, value); }

    /// <summary>
    /// 选中的角色
    /// </summary>
    public Monster? Selected
    {
        get => selected; set
        {
            if (SetProperty(ref selected, value))
            {
                UpdateBaseValueInfo(value);
            }
        }
    }

    /// <summary>
    /// 基础数值信息
    /// </summary>
    public BaseValueInfo? BaseValueInfo { get => baseValueInfo; set => SetProperty(ref baseValueInfo, value); }

    protected override async ValueTask<bool> InitializeOverrideAsync()
    {
        if (await metadataService.InitializeAsync().ConfigureAwait(false))
        {
            try
            {
                levelMonsterCurveMap = await metadataService.GetLevelToMonsterCurveMapAsync().ConfigureAwait(false);

                List<Monster> monsters = await metadataService.GetMonsterListAsync().ConfigureAwait(false);
                Dictionary<MaterialId, DisplayItem> idDisplayMap = await metadataService.GetIdToDisplayItemAndMaterialMapAsync().ConfigureAwait(false);
                foreach (Monster monster in monsters)
                {
                    monster.DropsView ??= monster.Drops?.SelectList(i => idDisplayMap.GetValueOrDefault(i, Material.Default));
                }

                List<Monster> ordered = monsters.SortBy(m => m.RelationshipId.Value);

                using (await EnterCriticalSectionAsync().ConfigureAwait(false))
                {
                    AdvancedCollectionView<Monster> monstersView = ordered.ToAdvancedCollectionView();

                    await taskContext.SwitchToMainThreadAsync();
                    Monsters = monstersView;

                    // TODO: use CurrentItem
                    Selected = Monsters.View.ElementAtOrDefault(0);
                }

                return true;
            }
            catch (OperationCanceledException)
            {
            }
        }

        return false;
    }

    private void UpdateBaseValueInfo(Monster? monster)
    {
        if (monster is null)
        {
            BaseValueInfo = null;
        }
        else
        {
            List<PropertyCurveValue> propertyCurveValues = monster.GrowCurves
                .SelectList(curveInfo => new PropertyCurveValue(curveInfo.Type, curveInfo.Value, monster.BaseValue.GetValue(curveInfo.Type)));

            ArgumentNullException.ThrowIfNull(levelMonsterCurveMap);
            BaseValueInfo = new(Monster.MaxLevel, propertyCurveValues, levelMonsterCurveMap);
        }
    }
}