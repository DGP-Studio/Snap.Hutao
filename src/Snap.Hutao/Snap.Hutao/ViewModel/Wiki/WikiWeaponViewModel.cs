// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI.Collections;
using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.Model.Calculable;
using Snap.Hutao.Model.Entity.Primitive;
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Metadata;
using Snap.Hutao.Model.Metadata.Item;
using Snap.Hutao.Model.Metadata.Weapon;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.Service.Cultivation;
using Snap.Hutao.Service.Hutao;
using Snap.Hutao.Service.Metadata;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.Service.User;
using Snap.Hutao.View.Dialog;
using Snap.Hutao.Web.Response;
using System.Collections.Immutable;
using System.Runtime.InteropServices;
using CalculateAvatarPromotionDelta = Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate.AvatarPromotionDelta;
using CalculateClient = Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate.CalculateClient;
using CalculateConsumption = Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate.Consumption;

namespace Snap.Hutao.ViewModel.Wiki;

/// <summary>
/// 武器资料视图模型
/// </summary>
[ConstructorGenerated]
[Injection(InjectAs.Scoped)]
internal sealed partial class WikiWeaponViewModel : Abstraction.ViewModel
{
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly CalculateClient calculateClient;
    private readonly ICultivationService cultivationService;
    private readonly ITaskContext taskContext;
    private readonly IMetadataService metadataService;
    private readonly IHutaoCache hutaoCache;
    private readonly IInfoBarService infoBarService;
    private readonly IUserService userService;

    private AdvancedCollectionView? weapons;
    private Weapon? selected;
    private string? filterText;
    private BaseValueInfo? baseValueInfo;
    private Dictionary<Level, Dictionary<GrowCurveType, float>>? levelWeaponCurveMap;
    private List<Promote>? promotes;

    /// <summary>
    /// 角色列表
    /// </summary>
    public AdvancedCollectionView? Weapons { get => weapons; set => SetProperty(ref weapons, value); }

    /// <summary>
    /// 选中的角色
    /// </summary>
    public Weapon? Selected
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
            levelWeaponCurveMap = await metadataService.GetLevelToWeaponCurveMapAsync().ConfigureAwait(false);
            promotes = await metadataService.GetWeaponPromotesAsync().ConfigureAwait(false);
            Dictionary<MaterialId, Material> idMaterialMap = await metadataService.GetIdToMaterialMapAsync().ConfigureAwait(false);

            List<Weapon> weapons = await metadataService.GetWeaponsAsync().ConfigureAwait(false);
            List<Weapon> sorted = weapons
                .OrderByDescending(weapon => weapon.RankLevel)
                .ThenBy(weapon => weapon.WeaponType)
                .ThenByDescending(weapon => weapon.Id.Value)
                .ToList();

            await CombineComplexDataAsync(sorted, idMaterialMap).ConfigureAwait(false);

            await taskContext.SwitchToMainThreadAsync();

            Weapons = new AdvancedCollectionView(sorted, true);
            Selected = Weapons.Cast<Weapon>().FirstOrDefault();
        }
    }

    private async ValueTask CombineComplexDataAsync(List<Weapon> weapons, Dictionary<MaterialId, Material> idMaterialMap)
    {
        if (await hutaoCache.InitializeForWikiWeaponViewModelAsync().ConfigureAwait(false))
        {
            ArgumentNullException.ThrowIfNull(hutaoCache.WeaponCollocations);

            foreach (Weapon weapon in weapons)
            {
                weapon.Collocation = hutaoCache.WeaponCollocations.GetValueOrDefault(weapon.Id);
                weapon.CultivationItemsView ??= weapon.CultivationItems.SelectList(i => idMaterialMap.GetValueOrDefault(i, Material.Default));
            }
        }
    }

    [Command("CultivateCommand")]
    private async Task CultivateAsync(Weapon? weapon)
    {
        if (weapon is null)
        {
            return;
        }

        if (userService.Current is null)
        {
            infoBarService.Warning(SH.MustSelectUserAndUid);
            return;
        }

        CalculableOptions options = new(null, weapon.ToCalculable());
        CultivatePromotionDeltaDialog dialog = await contentDialogFactory.CreateInstanceAsync<CultivatePromotionDeltaDialog>(options).ConfigureAwait(false);
        (bool isOk, CalculateAvatarPromotionDelta delta) = await dialog.GetPromotionDeltaAsync().ConfigureAwait(false);

        if (!isOk)
        {
            return;
        }

        Response<CalculateConsumption> consumptionResponse = await calculateClient
            .ComputeAsync(userService.Current.Entity, delta)
            .ConfigureAwait(false);

        if (!consumptionResponse.IsOk())
        {
            return;
        }

        CalculateConsumption consumption = consumptionResponse.Data;
        try
        {
            bool saved = await cultivationService
                .SaveConsumptionAsync(CultivateType.Weapon, weapon.Id, consumption.WeaponConsume.EmptyIfNull())
                .ConfigureAwait(false);

            if (saved)
            {
                infoBarService.Success(SH.ViewModelCultivationEntryAddSuccess);
            }
            else
            {
                infoBarService.Warning(SH.ViewModelCultivationEntryAddWarning);
            }
        }
        catch (Core.ExceptionService.UserdataCorruptedException ex)
        {
            infoBarService.Error(ex, SH.ViewModelCultivationAddWarning);
        }
    }

    private void UpdateBaseValueInfo(Weapon? weapon)
    {
        if (weapon is null)
        {
            BaseValueInfo = null;
            return;
        }

        ArgumentNullException.ThrowIfNull(promotes);
        Dictionary<PromoteLevel, Promote> weaponPromoteMap = promotes.Where(p => p.Id == weapon.PromoteId).ToDictionary(p => p.Level);
        List<PropertyCurveValue> propertyCurveValues = weapon.GrowCurves
            .SelectList(curveInfo => new PropertyCurveValue(curveInfo.Type, curveInfo.Value, curveInfo.InitValue));

        ArgumentNullException.ThrowIfNull(levelWeaponCurveMap);
        BaseValueInfo = new(weapon.MaxLevel, propertyCurveValues, levelWeaponCurveMap, weaponPromoteMap);
    }

    [Command("FilterCommand")]
    private void ApplyFilter(string? input)
    {
        if (Weapons is null)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(input))
        {
            Weapons.Filter = default!;
            return;
        }

        Weapons.Filter = WeaponFilter.Compile(input);

        if (Selected is not null && Weapons.Contains(Selected))
        {
            return;
        }

        try
        {
            Weapons.MoveCurrentToFirst();
        }
        catch (COMException)
        {
        }
    }
}