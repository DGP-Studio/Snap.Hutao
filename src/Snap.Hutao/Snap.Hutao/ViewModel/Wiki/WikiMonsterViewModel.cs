// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI.UI;
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Metadata.Item;
using Snap.Hutao.Model.Metadata.Monster;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.Service.Metadata;
using System.Collections.Immutable;

namespace Snap.Hutao.ViewModel.Wiki;

/// <summary>
/// 怪物资料视图模型
/// </summary>
[Injection(InjectAs.Scoped)]
internal class WikiMonsterViewModel : Abstraction.ViewModel
{
    private readonly IMetadataService metadataService;
    private readonly ITaskContext taskContext;

    private AdvancedCollectionView? monsters;
    private Monster? selected;
    private string? filterText;
    private BaseValueInfo? baseValueInfo;
    private Dictionary<int, Dictionary<GrowCurveType, float>>? levelMonsterCurveMap;

    /// <summary>
    /// 构造一个新的怪物资料视图模型
    /// </summary>
    /// <param name="serviceProvider">服务提供器</param>
    public WikiMonsterViewModel(IServiceProvider serviceProvider)
    {
        taskContext = serviceProvider.GetRequiredService<ITaskContext>();
        metadataService = serviceProvider.GetRequiredService<IMetadataService>();
    }

    /// <summary>
    /// 角色列表
    /// </summary>
    public AdvancedCollectionView? Monsters { get => monsters; set => SetProperty(ref monsters, value); }

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

    /// <summary>
    /// 筛选文本
    /// </summary>
    public string? FilterText { get => filterText; set => SetProperty(ref filterText, value); }

    /// <inheritdoc/>
    protected override async Task OpenUIAsync()
    {
        if (await metadataService.InitializeAsync().ConfigureAwait(false))
        {
            levelMonsterCurveMap = await metadataService.GetLevelToMonsterCurveMapAsync().ConfigureAwait(false);

            List<Monster> monsters = await metadataService.GetMonstersAsync().ConfigureAwait(false);
            Dictionary<MaterialId, Display> idDisplayMap = await metadataService.GetIdToDisplayAndMaterialMapAsync().ConfigureAwait(false);
            foreach (Monster monster in monsters)
            {
                monster.DropsView ??= monster.Drops?.SelectList(i => idDisplayMap.GetValueOrDefault(i)!);
            }

            List<Monster> ordered = monsters.OrderBy(m => m.Id.Value).ToList();
            await taskContext.SwitchToMainThreadAsync();

            Monsters = new AdvancedCollectionView(ordered, true);
            Selected = Monsters.Cast<Monster>().FirstOrDefault();
        }
    }

    private void UpdateBaseValueInfo(Monster? monster)
    {
        if (monster == null)
        {
            BaseValueInfo = null;
        }
        else
        {
            List<PropertyCurveValue> propertyCurveValues = monster.GrowCurves
                .Select(curveInfo => new PropertyCurveValue(curveInfo.Key, curveInfo.Value, monster.BaseValue.GetValue(curveInfo.Key)))
                .ToList();

            BaseValueInfo = new(100, propertyCurveValues, levelMonsterCurveMap!);
        }
    }
}