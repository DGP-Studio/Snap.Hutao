// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.WinUI.UI;
using Microsoft.Extensions.Primitives;
using Snap.Hutao.Model.Entity.Primitive;
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Intrinsic.Immutable;
using Snap.Hutao.Model.Metadata;
using Snap.Hutao.Model.Metadata.Weapon;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.Service.Abstraction;
using Snap.Hutao.Service.Cultivation;
using Snap.Hutao.Service.Hutao;
using Snap.Hutao.Service.Metadata;
using Snap.Hutao.Service.User;
using Snap.Hutao.View.Dialog;
using Snap.Hutao.ViewModel.Complex;
using Snap.Hutao.Web.Response;
using System.Collections.Immutable;
using System.Runtime.InteropServices;
using CalcAvatarPromotionDelta = Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate.AvatarPromotionDelta;
using CalcClient = Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate.CalculateClient;
using CalcConsumption = Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate.Consumption;

namespace Snap.Hutao.ViewModel.Wiki;

/// <summary>
/// 武器资料视图模型
/// </summary>
[Injection(InjectAs.Scoped)]
internal class WikiWeaponViewModel : Abstraction.ViewModel
{
    private readonly List<WeaponId> skippedWeapons = new()
    {
        12304, 14306, 15306, 13304, // 石英大剑, 琥珀玥, 黑檀弓, 「旗杆」
        11419, 11420, 11421, // 「一心传」名刀
    };

    private readonly ITaskContext taskContext;
    private readonly IServiceProvider serviceProvider;
    private readonly IMetadataService metadataService;
    private readonly IHutaoCache hutaoCache;

    private AdvancedCollectionView? weapons;
    private Weapon? selected;
    private string? filterText;
    private BaseValueInfo? baseValueInfo;
    private Dictionary<int, Dictionary<GrowCurveType, float>>? levelWeaponCurveMap;
    private List<Promote>? promotes;

    /// <summary>
    /// 构造一个新的武器资料视图模型
    /// </summary>
    /// <param name="serviceProvider">服务提供器</param>
    public WikiWeaponViewModel(IServiceProvider serviceProvider)
    {
        taskContext = serviceProvider.GetRequiredService<ITaskContext>();
        metadataService = serviceProvider.GetRequiredService<IMetadataService>();
        hutaoCache = serviceProvider.GetRequiredService<IHutaoCache>();
        this.serviceProvider = serviceProvider;

        CultivateCommand = new AsyncRelayCommand<Weapon>(CultivateAsync);
        FilterCommand = new RelayCommand<string>(ApplyFilter);
    }

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

    /// <summary>
    /// 养成命令
    /// </summary>
    public ICommand CultivateCommand { get; }

    /// <summary>
    /// 筛选命令
    /// </summary>
    public ICommand FilterCommand { get; }

    /// <inheritdoc/>
    protected override async Task OpenUIAsync()
    {
        if (await metadataService.InitializeAsync().ConfigureAwait(false))
        {
            levelWeaponCurveMap = await metadataService.GetLevelToWeaponCurveMapAsync().ConfigureAwait(false);
            promotes = await metadataService.GetWeaponPromotesAsync().ConfigureAwait(false);

            List<Weapon> weapons = await metadataService.GetWeaponsAsync().ConfigureAwait(false);
            List<Weapon> sorted = weapons
                .Where(weapon => !skippedWeapons.Contains(weapon.Id))
                .OrderByDescending(weapon => weapon.RankLevel)
                .ThenBy(weapon => weapon.WeaponType)
                .ThenByDescending(weapon => weapon.Id.Value)
                .ToList();

            await CombineWithWeaponCollocationsAsync(sorted).ConfigureAwait(false);

            await taskContext.SwitchToMainThreadAsync();

            Weapons = new AdvancedCollectionView(sorted, true);
            Selected = Weapons.Cast<Weapon>().FirstOrDefault();
        }
    }

    private async Task CombineWithWeaponCollocationsAsync(List<Weapon> weapons)
    {
        if (await hutaoCache.InitializeForWikiWeaponViewModelAsync().ConfigureAwait(false))
        {
            Dictionary<WeaponId, WeaponCollocationView> idCollocations = hutaoCache.WeaponCollocations!.ToDictionary(a => a.WeaponId);
            weapons.ForEach(w => w.Collocation = idCollocations.GetValueOrDefault(w.Id));
        }
    }

    private async Task CultivateAsync(Weapon? weapon)
    {
        if (weapon != null)
        {
            IInfoBarService infoBarService = serviceProvider.GetRequiredService<IInfoBarService>();
            IUserService userService = serviceProvider.GetRequiredService<IUserService>();

            if (userService.Current != null)
            {
                // ContentDialog must be created by main thread.
                await taskContext.SwitchToMainThreadAsync();
                CultivatePromotionDeltaDialog dialog = serviceProvider.CreateInstance<CultivatePromotionDeltaDialog>(weapon.ToCalculable());
                (bool isOk, CalcAvatarPromotionDelta delta) = await dialog.GetPromotionDeltaAsync().ConfigureAwait(false);

                if (isOk)
                {
                    Response<CalcConsumption> consumptionResponse = await serviceProvider
                        .GetRequiredService<CalcClient>()
                        .ComputeAsync(userService.Current.Entity, delta)
                        .ConfigureAwait(false);

                    if (consumptionResponse.IsOk())
                    {
                        CalcConsumption consumption = consumptionResponse.Data;

                        try
                        {
                            bool saved = await serviceProvider
                                .GetRequiredService<ICultivationService>()
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
                }
            }
            else
            {
                infoBarService.Warning(SH.MustSelectUserAndUid);
            }
        }
    }

    private void UpdateBaseValueInfo(Weapon? weapon)
    {
        if (weapon == null)
        {
            BaseValueInfo = null;
        }
        else
        {
            Dictionary<int, Promote> weaponPromoteMap = promotes!.Where(p => p.Id == weapon.PromoteId).ToDictionary(p => p.Level);

            List<PropertyCurveValue> propertyCurveValues = weapon.GrowCurves
                .Select(curveInfo => new PropertyCurveValue(curveInfo.Key, curveInfo.Value.Type, curveInfo.Value.Value))
                .ToList();

            BaseValueInfo = new(weapon.MaxLevel, propertyCurveValues, levelWeaponCurveMap!, weaponPromoteMap);
        }
    }

    private void ApplyFilter(string? input)
    {
        if (Weapons != null)
        {
            if (!string.IsNullOrWhiteSpace(input))
            {
                Weapons.Filter = WeaponFilter.Compile(input);

                if (!Weapons.Contains(Selected))
                {
                    try
                    {
                        Weapons.MoveCurrentToFirst();
                    }
                    catch (COMException)
                    {
                    }
                }
            }
            else
            {
                Weapons.Filter = null!;
            }
        }
    }

    private static class WeaponFilter
    {
        public static Predicate<object> Compile(string input)
        {
            return (object o) => o is Weapon weapon && DoFilter(input, weapon);
        }

        private static bool DoFilter(string input, Weapon weapon)
        {
            List<bool> matches = new();

            foreach (StringSegment segment in new StringTokenizer(input, ' '.Enumerate().ToArray()))
            {
                string value = segment.ToString();

                if (weapon.Name == value)
                {
                    matches.Add(true);
                    continue;
                }

                if (IntrinsicImmutables.WeaponTypes.Contains(value))
                {
                    matches.Add(weapon.WeaponType.GetLocalizedDescriptionOrDefault() == value);
                    continue;
                }

                if (IntrinsicImmutables.ItemQualities.Contains(value))
                {
                    matches.Add(weapon.Quality.GetLocalizedDescriptionOrDefault() == value);
                    continue;
                }

                if (IntrinsicImmutables.FightProperties.Contains(value))
                {
                    matches.Add(weapon.GrowCurves.ElementAtOrDefault(1).Key.GetLocalizedDescriptionOrDefault() == value);
                    continue;
                }
            }

            return matches.Count > 0 && matches.Aggregate((a, b) => a && b);
        }
    }
}