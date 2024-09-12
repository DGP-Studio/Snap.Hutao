// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.ExceptionService;
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
internal sealed partial class WikiAvatarViewModel : Abstraction.ViewModel
{
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly ICultivationService cultivationService;
    private readonly IMetadataService metadataService;
    private readonly ITaskContext taskContext;
    private readonly IHutaoSpiralAbyssStatisticsCache hutaoCache;
    private readonly IServiceScopeFactory serviceScopeFactory;
    private readonly IInfoBarService infoBarService;
    private readonly IUserService userService;

    private AdvancedCollectionView<Avatar>? avatars;
    private ObservableCollection<SearchToken>? filterTokens;
    private string? filterToken;
    private BaseValueInfo? baseValueInfo;
    private Dictionary<Level, Dictionary<GrowCurveType, float>>? levelAvatarCurveMap;
    private List<Promote>? promotes;
    private FrozenDictionary<string, SearchToken> availableTokens;

    public AdvancedCollectionView<Avatar>? Avatars
    {
        get => avatars;
        set
        {
            if (Avatars is not null)
            {
                Avatars.CurrentChanged -= OnCurrentAvatarChanged;
            }

            SetProperty(ref avatars, value);

            if (value is not null)
            {
                value.CurrentChanged += OnCurrentAvatarChanged;
            }
        }
    }

    public BaseValueInfo? BaseValueInfo { get => baseValueInfo; set => SetProperty(ref baseValueInfo, value); }

    public ObservableCollection<SearchToken>? FilterTokens { get => filterTokens; set => SetProperty(ref filterTokens, value); }

    public string? FilterToken { get => filterToken; set => SetProperty(ref filterToken, value); }

    public FrozenDictionary<string, SearchToken>? AvailableTokens { get => availableTokens; }

    protected override async ValueTask<bool> InitializeOverrideAsync()
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

                using (await EnterCriticalSectionAsync().ConfigureAwait(false))
                {
                    AdvancedCollectionView<Avatar> avatarsView = list.ToAdvancedCollectionView();

                    await taskContext.SwitchToMainThreadAsync();
                    Avatars = avatarsView;
                    Avatars.MoveCurrentToFirstOrDefault();
                }

                FilterTokens = [];

                availableTokens = FrozenDictionary.ToFrozenDictionary(
                [
                    .. avatars.Select((avatar, index) => KeyValuePair.Create(avatar.Name, new SearchToken(SearchTokenKind.Avatar, avatar.Name, index, sideIconUri: AvatarSideIconConverter.IconNameToUri(avatar.SideIcon)))),
                    .. IntrinsicFrozen.AssociationTypeNameValues.Select(nv => KeyValuePair.Create(nv.Name, new SearchToken(SearchTokenKind.AssociationType, nv.Name, (int)nv.Value, iconUri: AssociationTypeIconConverter.AssociationTypeToIconUri(nv.Value)))),
                    .. IntrinsicFrozen.BodyTypeNameValues.Select(nv => KeyValuePair.Create(nv.Name, new SearchToken(SearchTokenKind.BodyType, nv.Name, (int)nv.Value))),
                    .. IntrinsicFrozen.ElementNameValues.Select(nv => KeyValuePair.Create(nv.Name, new SearchToken(SearchTokenKind.ElementName, nv.Name, nv.Value, iconUri: ElementNameIconConverter.ElementNameToIconUri(nv.Name)))),
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

    private void OnCurrentAvatarChanged(object? sender, object? e)
    {
        UpdateBaseValueInfo(Avatars?.CurrentItem);
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
            avatar.CollocationView = hutaoCache.AvatarCollocations.GetValueOrDefault(avatar.Id);
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

        if (await userService.GetCurrentUserAndUidAsync().ConfigureAwait(false) is not { } userAndUid)
        {
            infoBarService.Warning(SH.MustSelectUserAndUid);
            return;
        }

        CalculableOptions options = new(avatar.ToCalculable(), null);
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
                Type = CultivateType.AvatarAndSkill,
                ItemId = avatar.Id,
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

        if (FilterTokens is null or [])
        {
            Avatars.Filter = default!;
        }
        else
        {
            Avatars.Filter = AvatarFilter.Compile(FilterTokens);
        }
    }
}