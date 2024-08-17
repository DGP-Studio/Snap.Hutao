// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.Model.Calculable;
using Snap.Hutao.Model.Entity.Primitive;
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Intrinsic.Frozen;
using Snap.Hutao.Model.Metadata;
using Snap.Hutao.Model.Metadata.Converter;
using Snap.Hutao.Model.Metadata.Item;
using Snap.Hutao.Model.Metadata.Weapon;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.Service.Cultivation;
using Snap.Hutao.Service.Hutao;
using Snap.Hutao.Service.Metadata;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.Service.User;
using Snap.Hutao.UI.Xaml.Control.AutoSuggestBox;
using Snap.Hutao.UI.Xaml.Data;
using Snap.Hutao.UI.Xaml.View.Dialog;
using Snap.Hutao.Web.Response;
using System.Collections.Frozen;
using System.Collections.ObjectModel;
using CalculateBatchConsumption = Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate.BatchConsumption;
using CalculateClient = Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate.CalculateClient;

namespace Snap.Hutao.ViewModel.Wiki;

[ConstructorGenerated]
[Injection(InjectAs.Scoped)]
internal sealed partial class WikiWeaponViewModel : Abstraction.ViewModel
{
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly ICultivationService cultivationService;
    private readonly ITaskContext taskContext;
    private readonly IMetadataService metadataService;
    private readonly IHutaoSpiralAbyssStatisticsCache hutaoCache;
    private readonly IServiceScopeFactory serviceScopeFactory;
    private readonly IInfoBarService infoBarService;
    private readonly IUserService userService;

    private AdvancedCollectionView<Weapon>? weapons;
    private ObservableCollection<SearchToken>? filterTokens;
    private string? filterToken;
    private BaseValueInfo? baseValueInfo;
    private Dictionary<Level, Dictionary<GrowCurveType, float>>? levelWeaponCurveMap;
    private List<Promote>? promotes;
    private FrozenDictionary<string, SearchToken> availableTokens;

    public AdvancedCollectionView<Weapon>? Weapons
    {
        get => weapons;
        set
        {
            if (weapons is not null)
            {
                weapons.CurrentChanged -= OnCurrentWeaponChanged;
            }

            SetProperty(ref weapons, value);

            if (value is not null)
            {
                value.CurrentChanged += OnCurrentWeaponChanged;
            }
        }
    }

    public BaseValueInfo? BaseValueInfo { get => baseValueInfo; set => SetProperty(ref baseValueInfo, value); }

    public ObservableCollection<SearchToken>? FilterTokens { get => filterTokens; set => SetProperty(ref filterTokens, value); }

    public string? FilterToken { get => filterToken; set => SetProperty(ref filterToken, value); }

    public FrozenDictionary<string, SearchToken>? AvailableTokens { get => availableTokens; }

    /// <inheritdoc/>
    protected override async ValueTask<bool> InitializeOverrideAsync()
    {
        if (await metadataService.InitializeAsync().ConfigureAwait(false))
        {
            try
            {
                levelWeaponCurveMap = await metadataService.GetLevelToWeaponCurveMapAsync().ConfigureAwait(false);
                promotes = await metadataService.GetWeaponPromoteListAsync().ConfigureAwait(false);
                Dictionary<MaterialId, Material> idMaterialMap = await metadataService.GetIdToMaterialMapAsync().ConfigureAwait(false);

                List<Weapon> weapons = await metadataService.GetWeaponListAsync().ConfigureAwait(false);
                IEnumerable<Weapon> sorted = weapons
                    .OrderByDescending(weapon => weapon.Sort);
                List<Weapon> list = [.. sorted];

                await CombineComplexDataAsync(list, idMaterialMap).ConfigureAwait(false);

                using (await EnterCriticalSectionAsync().ConfigureAwait(false))
                {
                    AdvancedCollectionView<Weapon> weaponsView = list.ToAdvancedCollectionView();

                    await taskContext.SwitchToMainThreadAsync();
                    Weapons = weaponsView;
                    Weapons.MoveCurrentToFirstOrDefault();
                }

                FilterTokens = [];

                availableTokens = FrozenDictionary.ToFrozenDictionary(
                [
                    .. weapons.Select((weapon, index) => KeyValuePair.Create(weapon.Name, new SearchToken(SearchTokenKind.Weapon, weapon.Name, index, sideIconUri: EquipIconConverter.IconNameToUri(weapon.Icon)))),
                    .. IntrinsicFrozen.FightPropertyNameValues.Select(nv => KeyValuePair.Create(nv.Name, new SearchToken(SearchTokenKind.FightProperty, nv.Name, (int)nv.Value))),
                    .. IntrinsicFrozen.ItemQualityNameValues.Select(nv => KeyValuePair.Create(nv.Name, new SearchToken(SearchTokenKind.ItemQuality, nv.Name, (int)nv.Value, quality: QualityColorConverter.QualityToColor(nv.Value)))),
                    .. IntrinsicFrozen.WeaponTypeNameValues.Select(nv => KeyValuePair.Create(nv.Name, new SearchToken(SearchTokenKind.WeaponType, nv.Name, (int)nv.Value, iconUri: WeaponTypeIconConverter.WeaponTypeToIconUri(nv.Value)))),
                ]);

                return true;
            }
            catch (OperationCanceledException)
            {
            }
        }

        return false;
    }

    private void OnCurrentWeaponChanged(object? sender, object? e)
    {
        UpdateBaseValueInfo(Weapons?.CurrentItem);
    }

    private async ValueTask CombineComplexDataAsync(List<Weapon> weapons, Dictionary<MaterialId, Material> idMaterialMap)
    {
        if (await hutaoCache.InitializeForWikiWeaponViewAsync().ConfigureAwait(false))
        {
            ArgumentNullException.ThrowIfNull(hutaoCache.WeaponCollocations);

            foreach (Weapon weapon in weapons)
            {
                weapon.CollocationView = hutaoCache.WeaponCollocations.GetValueOrDefault(weapon.Id);
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

        if (await userService.GetCurrentUserAndUidAsync().ConfigureAwait(false) is not { } userAndUid)
        {
            infoBarService.Warning(SH.MustSelectUserAndUid);
            return;
        }

        CalculableOptions options = new(null, weapon.ToCalculable());
        CultivatePromotionDeltaDialog dialog = await contentDialogFactory.CreateInstanceAsync<CultivatePromotionDeltaDialog>(options).ConfigureAwait(false);
        (bool isOk, CultivatePromotionDeltaOptions deltaOptions) = await dialog.GetPromotionDeltaAsync().ConfigureAwait(false);

        if (!isOk)
        {
            return;
        }

        Response<CalculateBatchConsumption> response;
        using (IServiceScope scope = serviceScopeFactory.CreateScope())
        {
            CalculateClient calculateClient = scope.ServiceProvider.GetRequiredService<CalculateClient>();
            response = await calculateClient.BatchComputeAsync(userAndUid, deltaOptions.Delta).ConfigureAwait(false);
        }

        if (!response.IsOk())
        {
            return;
        }

        CalculateBatchConsumption batchConsumption = response.Data;
        LevelInformation levelInformation = LevelInformation.From(deltaOptions.Delta);
        try
        {
            InputConsumption input = new()
            {
                Type = CultivateType.Weapon,
                ItemId = weapon.Id,
                Items = batchConsumption.OverallConsume,
                LevelInformation = levelInformation,
                Strategy = deltaOptions.Strategy,
            };

            switch (await cultivationService.SaveConsumptionAsync(input).ConfigureAwait(false))
            {
                case ConsumptionSaveResultKind.NoProject:
                    infoBarService.Warning(SH.ViewModelCultivationEntryAddWarning);
                    break;
                case ConsumptionSaveResultKind.Skipped:
                    infoBarService.Information(SH.ViewModelCultivationConsumptionSaveSkippedHint);
                    break;
                case ConsumptionSaveResultKind.NoItem:
                    infoBarService.Information(SH.ViewModelCultivationConsumptionSaveNoItemHint);
                    break;
                case ConsumptionSaveResultKind.Added:
                    infoBarService.Success(SH.ViewModelCultivationEntryAddSuccess);
                    break;
            }
        }
        catch (HutaoException ex)
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
    private void ApplyFilter()
    {
        if (Weapons is null)
        {
            return;
        }

        if (FilterTokens is null or [])
        {
            Weapons.Filter = default!;
        }
        else
        {
            Weapons.Filter = WeaponFilter.Compile(FilterTokens);
        }
    }
}