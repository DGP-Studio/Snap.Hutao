// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Core.Logging;
using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.Model.Calculable;
using Snap.Hutao.Model.Entity.Primitive;
using Snap.Hutao.Model.Metadata;
using Snap.Hutao.Model.Metadata.Avatar;
using Snap.Hutao.Model.Metadata.Item;
using Snap.Hutao.Service.Cultivation;
using Snap.Hutao.Service.Cultivation.Consumption;
using Snap.Hutao.Service.Cultivation.Offline;
using Snap.Hutao.Service.Hutao;
using Snap.Hutao.Service.Metadata;
using Snap.Hutao.Service.Metadata.ContextAbstraction;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.UI.Xaml.Control.AutoSuggestBox;
using Snap.Hutao.UI.Xaml.Data;
using Snap.Hutao.UI.Xaml.View.Dialog;
using System.Collections.Immutable;
using CalculateBatchConsumption = Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate.BatchConsumption;

namespace Snap.Hutao.ViewModel.Wiki;

[BindableCustomPropertyProvider]
[Service(ServiceLifetime.Scoped)]
internal sealed partial class WikiAvatarViewModel : Abstraction.ViewModel
{
    private readonly IHutaoSpiralAbyssStatisticsCache hutaoCache;
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly IServiceScopeFactory serviceScopeFactory;
    private readonly ICultivationService cultivationService;
    private readonly IMetadataService metadataService;
    private readonly ITaskContext taskContext;
    private readonly IMessenger messenger;

    private WikiAvatarMetadataContext? metadataContext;

    [GeneratedConstructor]
    public partial WikiAvatarViewModel(IServiceProvider serviceProvider);

    public partial WikiAvatarStrategyComponent StrategyComponent { get; }

    public IAdvancedCollectionView<Avatar>? Avatars
    {
        get;
        set
        {
            AdvancedCollectionViewCurrentChanged.Detach(field, OnCurrentAvatarChanged);
            SetProperty(ref field, value);
            AdvancedCollectionViewCurrentChanged.Attach(field, OnCurrentAvatarChanged);
        }
    }

    [ObservableProperty]
    public partial BaseValueInfo? BaseValueInfo { get; set; }

    [ObservableProperty]
    public partial SearchData? SearchData { get; set; }

    [ObservableProperty]
    public partial LinkMetadataContext? LinkContext { get; set; }

    protected override async ValueTask<bool> LoadOverrideAsync(CancellationToken token)
    {
        if (!await metadataService.InitializeAsync().ConfigureAwait(false))
        {
            return false;
        }

        metadataContext = await metadataService.GetContextAsync<WikiAvatarMetadataContext>(token).ConfigureAwait(false);
        ImmutableArray<Avatar> avatars = [.. metadataContext.Avatars.OrderByDescending(avatar => avatar.BeginTime).ThenByDescending(avatar => avatar.Sort)];
        SearchData searchData = SearchData.CreateForWikiAvatar(avatars);
        await CombineComplexDataAsync(avatars, metadataContext).ConfigureAwait(false);

        using (await EnterCriticalSectionAsync().ConfigureAwait(false))
        {
            IAdvancedCollectionView<Avatar> avatarsView = avatars.AsAdvancedCollectionView();

            await taskContext.SwitchToMainThreadAsync();
            token.ThrowIfCancellationRequested();

            SearchData = searchData;
            Avatars = avatarsView;
            Avatars.MoveCurrentToFirst();
        }

        return true;
    }

    private void OnCurrentAvatarChanged(object? sender, object? e)
    {
        UpdateBaseValueInfo(Avatars?.CurrentItem);
        UpdateLinkContext(Avatars?.CurrentItem);
        Avatars?.CurrentItem?.CostumesView?.MoveCurrentToFirst();
    }

    private async ValueTask CombineComplexDataAsync(ImmutableArray<Avatar> avatars, WikiAvatarMetadataContext context)
    {
        HutaoSpiralAbyssStatisticsMetadataContext context2 = await metadataService.GetContextAsync<HutaoSpiralAbyssStatisticsMetadataContext>().ConfigureAwait(false);
        await hutaoCache.InitializeForWikiAvatarViewAsync(context2).ConfigureAwait(false);
        ArgumentNullException.ThrowIfNull(hutaoCache.AvatarCollocations);

        foreach (Avatar avatar in avatars)
        {
            avatar.CollocationView = hutaoCache.AvatarCollocations.GetValueOrDefault(avatar.Id);
            avatar.CookBonusView ??= CookBonusView.Create(avatar.FetterInfo.CookBonus, context.IdMaterialMap);
            avatar.CultivationItemsView ??= [.. avatar.CultivationItems.Select(i => context.IdMaterialMap.GetValueOrDefault(i, Material.Default))];
            avatar.CostumesView ??= avatar.Costumes.OrderByDescending(c => c.IsDefault).AsAdvancedCollectionView();
        }
    }

    [Command("CultivateCommand")]
    private async Task CultivateAsync(Avatar? avatar)
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Cultivate", "WikiAvatarViewModel.Command"));

        if (avatar is null)
        {
            return;
        }

        CultivatePromotionDeltaOptions deltaOptions;
        using (IServiceScope scope = serviceScopeFactory.CreateScope())
        {
            CalculableOptions options = new(avatar.ToCalculable(), null);
            CultivatePromotionDeltaDialog dialog = await contentDialogFactory.CreateInstanceAsync<CultivatePromotionDeltaDialog>(scope.ServiceProvider, options).ConfigureAwait(false);
            (bool isOk, deltaOptions) = await dialog.GetPromotionDeltaAsync().ConfigureAwait(false);

            if (!isOk)
            {
                return;
            }
        }

        CalculateBatchConsumption batchConsumption = OfflineCalculator.CalculateWikiAvatarConsumption(deltaOptions.Delta, avatar);
        if (batchConsumption.OverallConsume.IsEmpty)
        {
            messenger.Send(InfoBarMessage.Warning(SH.ViewModelCultivationEntryAddNoConsumptionWarning));
            return;
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

            InfoBarMessage? message = await cultivationService.SaveConsumptionAsync(input).ConfigureAwait(false) switch
            {
                ConsumptionSaveResultKind.NoProject => InfoBarMessage.Warning(SH.ViewModelCultivationEntryAddWarning),
                ConsumptionSaveResultKind.Skipped => InfoBarMessage.Information(SH.ViewModelCultivationConsumptionSaveSkippedHint),
                ConsumptionSaveResultKind.NoItem => InfoBarMessage.Information(SH.ViewModelCultivationConsumptionSaveNoItemHint),
                ConsumptionSaveResultKind.Added => InfoBarMessage.Success(SH.ViewModelCultivationEntryAddSuccess),
                _ => default,
            };

            if (message is not null)
            {
                messenger.Send(message);
            }
        }
        catch (HutaoException ex)
        {
            messenger.Send(InfoBarMessage.Error(SH.ViewModelCultivationAddWarning, ex));
        }
    }

    private void UpdateBaseValueInfo(Avatar? avatar)
    {
        if (avatar is null || metadataContext is null)
        {
            BaseValueInfo = null;
            return;
        }

        BaseValueInfoMetadataContext context = new()
        {
            GrowCurveMap = metadataContext.LevelDictionaryAvatarGrowCurveMap,
            PromoteMap = metadataContext.IdDictionaryAvatarLevelPromoteMap[avatar.PromoteId],
        };

        BaseValueInfo = new(avatar.MaxLevel, avatar.GrowCurves.ToPropertyCurveValues(avatar.BaseValue), context);
    }

    private void UpdateLinkContext(Avatar? avatar)
    {
        if (avatar is null || metadataContext is null)
        {
            LinkContext = null;
            return;
        }

        LinkContext = new()
        {
            IdNameMap = metadataContext.IdHyperLinkNameMap,
            Inherents = avatar.SkillDepot.Inherents,
            Skills = avatar.SkillDepot.CompositeSkillsNoInherents,
        };
    }

    [Command("FilterCommand")]
    private void ApplyFilter()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Filter avatars", "WikiAvatarViewModel.Command"));

        if (Avatars is null)
        {
            return;
        }

        Avatars.Filter = AvatarFilter.Compile(SearchData);

        if (Avatars.CurrentItem is null)
        {
            Avatars.MoveCurrentToFirst();
        }
    }
}