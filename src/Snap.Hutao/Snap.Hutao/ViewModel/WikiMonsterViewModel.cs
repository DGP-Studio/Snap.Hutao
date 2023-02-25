// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.WinUI.UI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using Snap.Hutao.Model.Binding.BaseValue;
using Snap.Hutao.Model.Binding.Hutao;
using Snap.Hutao.Model.Entity.Primitive;
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Intrinsic.Immutable;
using Snap.Hutao.Model.Metadata.Item;
using Snap.Hutao.Model.Metadata.Monster;
using Snap.Hutao.Model.Metadata.Weapon;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.Service.Abstraction;
using Snap.Hutao.Service.Cultivation;
using Snap.Hutao.Service.Hutao;
using Snap.Hutao.Service.Metadata;
using Snap.Hutao.Service.User;
using Snap.Hutao.View.Dialog;
using Snap.Hutao.Web.Response;
using System.Collections.Immutable;
using System.Runtime.InteropServices;
using CalcAvatarPromotionDelta = Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate.AvatarPromotionDelta;
using CalcClient = Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate.CalculateClient;
using CalcConsumption = Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate.Consumption;

namespace Snap.Hutao.ViewModel;

/// <summary>
/// 怪物资料视图模型
/// </summary>
[Injection(InjectAs.Scoped)]
internal class WikiMonsterViewModel : Abstraction.ViewModel
{
    private readonly IServiceProvider serviceProvider;
    private readonly IMetadataService metadataService;
    private readonly IHutaoCache hutaoCache;

    private AdvancedCollectionView? monsters;
    private Monster? selected;
    private string? filterText;
    private BaseValueInfo? baseValueInfo;
    private Dictionary<int, Dictionary<GrowCurveType, float>>? levelMonsterCurveMap;
    private Dictionary<MaterialId, Material>? idMaterialMap;

    /// <summary>
    /// 构造一个新的怪物资料视图模型
    /// </summary>
    /// <param name="serviceProvider">服务提供器</param>
    public WikiMonsterViewModel(IServiceProvider serviceProvider)
    {
        metadataService = serviceProvider.GetRequiredService<IMetadataService>();
        hutaoCache = serviceProvider.GetRequiredService<IHutaoCache>();
        this.serviceProvider = serviceProvider;

        OpenUICommand = new AsyncRelayCommand(OpenUIAsync);
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

    /// <summary>
    /// 打开界面命令
    /// </summary>
    public ICommand OpenUICommand { get; }

    private async Task OpenUIAsync()
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
            await ThreadHelper.SwitchToMainThreadAsync();

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