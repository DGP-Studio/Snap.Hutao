// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Control.AutoSuggestBox;
using Snap.Hutao.Control.Collection.AdvancedCollectionView;
using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.Model.Calculable;
using Snap.Hutao.Model.Entity.Primitive;
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Intrinsic.Frozen;
using Snap.Hutao.Model.Metadata;
using Snap.Hutao.Model.Metadata.Avatar;
using Snap.Hutao.Model.Metadata.Converter;
using Snap.Hutao.Model.Metadata.Item;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.Service.Cultivation;
using Snap.Hutao.Service.Hutao;
using Snap.Hutao.Service.Metadata;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.Service.User;
using Snap.Hutao.View.Dialog;
using Snap.Hutao.Web.Response;
using System.Collections.Frozen;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using CalculateAvatarPromotionDelta = Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate.AvatarPromotionDelta;
using CalculateClient = Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate.CalculateClient;
using CalculateConsumption = Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate.Consumption;
using CalculateItem = Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate.Item;
using CalculateItemHelper = Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate.ItemHelper;

namespace Snap.Hutao.ViewModel.Wiki;

/// <summary>
/// 角色资料视图模型
/// </summary>
[HighQuality]
[ConstructorGenerated]
[Injection(InjectAs.Scoped)]
internal sealed partial class WikiAvatarViewModel : Abstraction.ViewModel
{
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly ICultivationService cultivationService;
    private readonly IMetadataService metadataService;
    private readonly ITaskContext taskContext;
    private readonly IHutaoSpiralAbyssStatisticsCache hutaoCache;
    private readonly IInfoBarService infoBarService;
    private readonly CalculateClient calculateClient;
    private readonly IUserService userService;

    private AdvancedCollectionView<Avatar>? avatars;
    private Avatar? selected;
    private ObservableCollection<SearchToken>? filterTokens;
    private string? filterToken;
    private BaseValueInfo? baseValueInfo;
    private Dictionary<Level, Dictionary<GrowCurveType, float>>? levelAvatarCurveMap;
    private List<Promote>? promotes;
    private FrozenDictionary<string, SearchToken> availableTokens;

    /// <summary>
    /// 角色列表
    /// </summary>
    public AdvancedCollectionView<Avatar>? Avatars { get => avatars; set => SetProperty(ref avatars, value); }

    /// <summary>
    /// 选中的角色
    /// </summary>
    public Avatar? Selected
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
    /// 保存的筛选标志
    /// </summary>
    public ObservableCollection<SearchToken>? FilterTokens { get => filterTokens; set => SetProperty(ref filterTokens, value); }

    public string? FilterToken { get => filterToken; set => SetProperty(ref filterToken, value); }

    public FrozenDictionary<string, SearchToken>? AvailableTokens { get => availableTokens; }

    protected override async ValueTask<bool> InitializeUIAsync()
    {
        if (await metadataService.InitializeAsync().ConfigureAwait(false))
        {
            try
            {
                levelAvatarCurveMap = await metadataService.GetLevelToAvatarCurveMapAsync().ConfigureAwait(false);
                promotes = await metadataService.GetAvatarPromoteListAsync().ConfigureAwait(false);

                Dictionary<MaterialId, Material> idMaterialMap = await metadataService.GetIdToMaterialMapAsync().ConfigureAwait(false);
                List<Avatar> avatars = await metadataService.GetAvatarListAsync().ConfigureAwait(false);
                IOrderedEnumerable<Avatar> sorted = avatars
                    .OrderByDescending(avatar => avatar.BeginTime)
                    .ThenByDescending(avatar => avatar.Sort);
                List<Avatar> list = [.. sorted];

                await CombineComplexDataAsync(list, idMaterialMap).ConfigureAwait(false);

                using (await EnterCriticalExecutionAsync().ConfigureAwait(false))
                {
                    await taskContext.SwitchToMainThreadAsync();
                    Avatars = new(list, true);
                    Selected = Avatars.View.ElementAtOrDefault(0);
                }

                FilterTokens = [];

                availableTokens = FrozenDictionary.ToFrozenDictionary(
                [
                    .. avatars.Select(avatar => KeyValuePair.Create(avatar.Name, new SearchToken(SearchTokenKind.Avatar, avatar.Name, sideIconUri: AvatarSideIconConverter.IconNameToUri(avatar.SideIcon)))),
                    .. IntrinsicFrozen.AssociationTypes.Select(assoc => KeyValuePair.Create(assoc, new SearchToken(SearchTokenKind.AssociationType, assoc, iconUri: AssociationTypeIconConverter.AssociationTypeNameToIconUri(assoc)))),
                    .. IntrinsicFrozen.BodyTypes.Select(b => KeyValuePair.Create(b, new SearchToken(SearchTokenKind.BodyType, b))),
                    .. IntrinsicFrozen.ElementNames.Select(e => KeyValuePair.Create(e, new SearchToken(SearchTokenKind.ElementName, e, iconUri: ElementNameIconConverter.ElementNameToIconUri(e)))),
                    .. IntrinsicFrozen.ItemQualities.Select(i => KeyValuePair.Create(i, new SearchToken(SearchTokenKind.ItemQuality, i, quality: QualityColorConverter.QualityNameToColor(i)))),
                    .. IntrinsicFrozen.WeaponTypes.Select(w => KeyValuePair.Create(w, new SearchToken(SearchTokenKind.WeaponType, w, iconUri: WeaponTypeIconConverter.WeaponTypeNameToIconUri(w)))),
                ]);

                return true;
            }
            catch (OperationCanceledException)
            {
            }
        }

        return false;
    }

    private async ValueTask CombineComplexDataAsync(List<Avatar> avatars, Dictionary<MaterialId, Material> idMaterialMap)
    {
        if (!await hutaoCache.InitializeForWikiAvatarViewAsync().ConfigureAwait(false))
        {
            return;
        }

        ArgumentNullException.ThrowIfNull(hutaoCache.AvatarCollocations);

        foreach (Avatar avatar in avatars)
        {
            avatar.Collocation = hutaoCache.AvatarCollocations.GetValueOrDefault(avatar.Id);
            avatar.CookBonusView ??= CookBonusView.Create(avatar.FetterInfo.CookBonus, idMaterialMap);
            avatar.CultivationItemsView ??= avatar.CultivationItems.SelectList(i => idMaterialMap.GetValueOrDefault(i, Material.Default));
        }
    }

    [Command("CultivateCommand")]
    private async Task CultivateAsync(Avatar? avatar)
    {
        if (avatar is null)
        {
            return;
        }

        if (userService.Current is null)
        {
            infoBarService.Warning(SH.MustSelectUserAndUid);
            return;
        }

        CalculableOptions options = new(avatar.ToCalculable(), null);
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
        LevelInformation levelInformation = LevelInformation.From(delta);
        List<CalculateItem> items = CalculateItemHelper.Merge(consumption.AvatarConsume, consumption.AvatarSkillConsume);
        try
        {
            bool saved = await cultivationService
                .SaveConsumptionAsync(CultivateType.AvatarAndSkill, avatar.Id, items, levelInformation)
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

    private void UpdateBaseValueInfo(Avatar? avatar)
    {
        if (avatar is null)
        {
            BaseValueInfo = null;
            return;
        }

        ArgumentNullException.ThrowIfNull(promotes);
        Dictionary<PromoteLevel, Promote> avatarPromoteMap = promotes.Where(p => p.Id == avatar.PromoteId).ToDictionary(p => p.Level);
        Dictionary<FightProperty, GrowCurveType> avatarGrowCurve = avatar.GrowCurves.ToDictionary(g => g.Type, g => g.Value);
        FightProperty promoteProperty = avatarPromoteMap[0].AddProperties.Last().Type;

        List<PropertyCurveValue> propertyCurveValues =
        [
            new(FightProperty.FIGHT_PROP_BASE_HP, avatarGrowCurve, avatar.BaseValue),
            new(FightProperty.FIGHT_PROP_BASE_ATTACK, avatarGrowCurve, avatar.BaseValue),
            new(FightProperty.FIGHT_PROP_BASE_DEFENSE, avatarGrowCurve, avatar.BaseValue),
            new(promoteProperty, avatarGrowCurve, avatar.BaseValue),
        ];

        ArgumentNullException.ThrowIfNull(levelAvatarCurveMap);
        BaseValueInfo = new(avatar.MaxLevel, propertyCurveValues, levelAvatarCurveMap, avatarPromoteMap);
    }

    [Command("FilterCommand")]
    private void ApplyFilter()
    {
        if (Avatars is null)
        {
            return;
        }

        if (FilterTokens.IsNullOrEmpty())
        {
            Avatars.Filter = default!;
        }
        else
        {
            Avatars.Filter = AvatarFilter.Compile(FilterTokens);
        }

        if (Selected is not null && Avatars.Contains(Selected))
        {
            return;
        }

        try
        {
            Avatars.MoveCurrentToFirst();
        }
        catch (COMException)
        {
        }
    }
}