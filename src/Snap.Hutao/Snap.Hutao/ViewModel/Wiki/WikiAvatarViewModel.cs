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
using Snap.Hutao.Service.Cultivation;
using Snap.Hutao.Service.Cultivation.Consumption;
using Snap.Hutao.Service.Hutao;
using Snap.Hutao.Service.Metadata;
using Snap.Hutao.Service.Metadata.ContextAbstraction;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.Service.User;
using Snap.Hutao.UI.Xaml.Control.AutoSuggestBox;
using Snap.Hutao.UI.Xaml.Data;
using Snap.Hutao.UI.Xaml.View.Dialog;
using Snap.Hutao.Web.Response;
using System.Collections.Frozen;
using System.Collections.Immutable;
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
    private WikiAvatarMetadataContext? metadataContext;
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

    protected override async ValueTask<bool> LoadOverrideAsync()
    {
        if (!await metadataService.InitializeAsync().ConfigureAwait(false))
        {
            return false;
        }

        try
        {
            metadataContext = await metadataService.GetContextAsync<WikiAvatarMetadataContext>().ConfigureAwait(false);

            List<Avatar> list = [.. metadataContext.Avatars.OrderByDescending(avatar => avatar.BeginTime).ThenByDescending(avatar => avatar.Sort)];

            await CombineComplexDataAsync(list, metadataContext).ConfigureAwait(false);

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
                .. list.Select((avatar, index) => KeyValuePair.Create(avatar.Name, new SearchToken(SearchTokenKind.Avatar, avatar.Name, index, sideIconUri: AvatarSideIconConverter.IconNameToUri(avatar.SideIcon)))),
                .. IntrinsicFrozen.AssociationTypeNameValues.Select(nv => KeyValuePair.Create(nv.Name, new SearchToken(SearchTokenKind.AssociationType, nv.Name, (int)nv.Value, iconUri: AssociationTypeIconConverter.AssociationTypeToIconUri(nv.Value)))),
                .. IntrinsicFrozen.BodyTypeNameValues.Select(nv => KeyValuePair.Create(nv.Name, new SearchToken(SearchTokenKind.BodyType, nv.Name, (int)nv.Value))),
                .. IntrinsicFrozen.ElementNameValues.Select(nv => KeyValuePair.Create(nv.Name, new SearchToken(SearchTokenKind.ElementName, nv.Name, nv.Value, iconUri: ElementNameIconConverter.ElementNameToIconUri(nv.Name)))),
                .. IntrinsicFrozen.ItemQualityNameValues.Where(nv => nv.Value >= QualityType.QUALITY_PURPLE).Select(nv => KeyValuePair.Create(nv.Name, new SearchToken(SearchTokenKind.ItemQuality, nv.Name, (int)nv.Value, quality: QualityColorConverter.QualityToColor(nv.Value)))),
                .. IntrinsicFrozen.WeaponTypeNameValues.Select(nv => KeyValuePair.Create(nv.Name, new SearchToken(SearchTokenKind.WeaponType, nv.Name, (int)nv.Value, iconUri: WeaponTypeIconConverter.WeaponTypeToIconUri(nv.Value)))),
            ]);

            return true;
        }
        catch (OperationCanceledException)
        {
        }

        return false;
    }

    private void OnCurrentAvatarChanged(object? sender, object? e)
    {
        UpdateBaseValueInfo(Avatars?.CurrentItem);
    }

    private async ValueTask CombineComplexDataAsync(List<Avatar> avatars, WikiAvatarMetadataContext context)
    {
        if (!await hutaoCache.InitializeForWikiAvatarViewAsync().ConfigureAwait(false))
        {
            return;
        }

        ArgumentNullException.ThrowIfNull(hutaoCache.AvatarCollocations);

        foreach (Avatar avatar in avatars)
        {
            avatar.CollocationView = hutaoCache.AvatarCollocations.GetValueOrDefault(avatar.Id);
            avatar.CookBonusView ??= CookBonusView.Create(avatar.FetterInfo.CookBonus, context.IdMaterialMap);
            avatar.CultivationItemsView ??= avatar.CultivationItems.SelectList(i => context.IdMaterialMap.GetValueOrDefault(i, Material.Default));
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

        CalculateBatchConsumption? batchConsumption;
        using (IServiceScope scope = serviceScopeFactory.CreateScope())
        {
            CalculateClient calculateClient = scope.ServiceProvider.GetRequiredService<CalculateClient>();
            Response<CalculateBatchConsumption> response = await calculateClient.BatchComputeAsync(userAndUid, deltaOptions.Delta).ConfigureAwait(false);

            if (!ResponseValidator.TryValidate(response, scope.ServiceProvider, out batchConsumption))
            {
                return;
            }
        }

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

            _ = await cultivationService.SaveConsumptionAsync(input).ConfigureAwait(false) switch
            {
                ConsumptionSaveResultKind.NoProject => infoBarService.Warning(SH.ViewModelCultivationEntryAddWarning),
                ConsumptionSaveResultKind.Skipped => infoBarService.Information(SH.ViewModelCultivationConsumptionSaveSkippedHint),
                ConsumptionSaveResultKind.NoItem => infoBarService.Information(SH.ViewModelCultivationConsumptionSaveNoItemHint),
                ConsumptionSaveResultKind.Added => infoBarService.Success(SH.ViewModelCultivationEntryAddSuccess),
                _ => default,
            };
        }
        catch (HutaoException ex)
        {
            infoBarService.Error(ex, SH.ViewModelCultivationAddWarning);
        }
    }

    private void UpdateBaseValueInfo(Avatar? avatar)
    {
        if (avatar is null || metadataContext is null)
        {
            BaseValueInfo = null;
            return;
        }

        BaseValueInfo = new(
            avatar.MaxLevel,
            avatar.GrowCurves.Select<TypeValue<FightProperty, GrowCurveType>, PropertyCurveValue>(info => new PropertyCurveValue(info.Type, info.Value, avatar.BaseValue.GetValue(info.Type))).ToList(),
            metadataContext.LevelDictionaryAvatarGrowCurveMap,
            metadataContext.IdDictionaryAvatarLevelPromoteMap[avatar.PromoteId]);
    }

    [Command("FilterCommand")]
    private void ApplyFilter()
    {
        if (Avatars is null)
        {
            return;
        }

        Avatars.Filter = FilterTokens is null or [] ? default! : AvatarFilter.Compile(FilterTokens);
    }
}