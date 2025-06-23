// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Core.Logging;
using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.Model.Calculable;
using Snap.Hutao.Model.Entity.Primitive;
using Snap.Hutao.Model.Metadata.Item;
using Snap.Hutao.Model.Metadata.Weapon;
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
using System.Collections.Immutable;
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

    private WikiWeaponMetadataContext? metadataContext;

    public IAdvancedCollectionView<Weapon>? Weapons
    {
        get;
        set
        {
            AdvancedCollectionViewCurrentChanged.Detach(field, OnCurrentWeaponChanged);
            SetProperty(ref field, value);
            AdvancedCollectionViewCurrentChanged.Attach(field, OnCurrentWeaponChanged);
        }
    }

    public BaseValueInfo? BaseValueInfo { get; set => SetProperty(ref field, value); }

    public SearchData? SearchData { get; set => SetProperty(ref field, value); }

    protected override async ValueTask<bool> LoadOverrideAsync(CancellationToken token)
    {
        if (!await metadataService.InitializeAsync().ConfigureAwait(false))
        {
            return false;
        }

        metadataContext = await metadataService.GetContextAsync<WikiWeaponMetadataContext>(token).ConfigureAwait(false);
        ImmutableArray<Weapon> weapons = [.. metadataContext.Weapons.OrderByDescending(weapon => weapon.Sort)];
        SearchData searchData = SearchData.CreateForWikiWeapon(weapons);

        await CombineComplexDataAsync(weapons, metadataContext).ConfigureAwait(false);

        using (await EnterCriticalSectionAsync().ConfigureAwait(false))
        {
            IAdvancedCollectionView<Weapon> weaponsView = weapons.AsAdvancedCollectionView();

            await taskContext.SwitchToMainThreadAsync();
            token.ThrowIfCancellationRequested();

            SearchData = searchData;
            Weapons = weaponsView;
            Weapons.MoveCurrentToFirst();
        }

        return true;
    }

    private void OnCurrentWeaponChanged(object? sender, object? e)
    {
        UpdateBaseValueInfo(Weapons?.CurrentItem);
    }

    private async ValueTask CombineComplexDataAsync(ImmutableArray<Weapon> weapons, WikiWeaponMetadataContext context)
    {
        HutaoSpiralAbyssStatisticsMetadataContext context2 = await metadataService.GetContextAsync<HutaoSpiralAbyssStatisticsMetadataContext>().ConfigureAwait(false);
        await hutaoCache.InitializeForWikiWeaponViewAsync(context2).ConfigureAwait(false);
        ArgumentNullException.ThrowIfNull(hutaoCache.WeaponCollocations);

        foreach (Weapon weapon in weapons)
        {
            weapon.CollocationView = hutaoCache.WeaponCollocations.GetValueOrDefault(weapon.Id);
            weapon.CultivationItemsView ??= [.. weapon.CultivationItems.Select(i => context.IdMaterialMap.GetValueOrDefault(i, Material.Default))];
        }
    }

    [Command("CultivateCommand")]
    private async Task CultivateAsync(Weapon? weapon)
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Cultivate", "WikiAvatarViewModel.Command"));

        if (weapon is null)
        {
            return;
        }

        if (await userService.GetCurrentUserAndUidAsync().ConfigureAwait(false) is not { } userAndUid)
        {
            infoBarService.Warning(SH.MustSelectUserAndUid);
            return;
        }

        CultivatePromotionDeltaOptions deltaOptions;
        using (IServiceScope scope = serviceScopeFactory.CreateScope())
        {
            CalculableOptions options = new(null, weapon.ToCalculable());
            CultivatePromotionDeltaDialog dialog = await contentDialogFactory.CreateInstanceAsync<CultivatePromotionDeltaDialog>(scope.ServiceProvider, options).ConfigureAwait(false);
            (bool isOk, deltaOptions) = await dialog.GetPromotionDeltaAsync().ConfigureAwait(false);

            if (!isOk)
            {
                return;
            }
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
                Type = CultivateType.Weapon,
                ItemId = weapon.Id,
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

    private void UpdateBaseValueInfo(Weapon? weapon)
    {
        if (weapon is null || metadataContext is null)
        {
            BaseValueInfo = null;
            return;
        }

        BaseValueInfoMetadataContext context = new()
        {
            GrowCurveMap = metadataContext.LevelDictionaryWeaponGrowCurveMap,
            PromoteMap = metadataContext.IdDictionaryWeaponLevelPromoteMap[weapon.PromoteId],
        };

        BaseValueInfo = new(weapon.MaxLevel, weapon.GrowCurves.ToPropertyCurveValues(), context);
    }

    [Command("FilterCommand")]
    private void ApplyFilter()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Filter weapons", "WikiAvatarViewModel.Command"));

        if (Weapons is null)
        {
            return;
        }

        Weapons.Filter = WeaponFilter.Compile(SearchData);

        if (Weapons.CurrentItem is null)
        {
            Weapons.MoveCurrentToFirst();
        }
    }
}