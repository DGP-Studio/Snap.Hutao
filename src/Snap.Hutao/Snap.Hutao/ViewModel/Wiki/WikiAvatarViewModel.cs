// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.WinUI.UI;
using Microsoft.Extensions.Primitives;
using Snap.Hutao.Model.Entity.Primitive;
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Intrinsic.Immutable;
using Snap.Hutao.Model.Metadata;
using Snap.Hutao.Model.Metadata.Avatar;
using Snap.Hutao.Model.Metadata.Item;
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
using CalcItem = Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate.Item;
using CalcItemHelper = Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate.ItemHelper;

namespace Snap.Hutao.ViewModel.Wiki;

/// <summary>
/// 角色资料视图模型
/// </summary>
[HighQuality]
[Injection(InjectAs.Scoped)]
internal sealed class WikiAvatarViewModel : Abstraction.ViewModel
{
    private readonly ITaskContext taskContext;
    private readonly IServiceProvider serviceProvider;
    private readonly IMetadataService metadataService;
    private readonly IHutaoCache hutaoCache;

    private AdvancedCollectionView? avatars;
    private Avatar? selected;
    private string? filterText;
    private BaseValueInfo? baseValueInfo;
    private Dictionary<int, Dictionary<GrowCurveType, float>>? levelAvatarCurveMap;
    private List<Promote>? promotes;

    /// <summary>
    /// 构造一个新的角色资料视图模型
    /// </summary>
    /// <param name="serviceProvider">服务提供器</param>
    public WikiAvatarViewModel(IServiceProvider serviceProvider)
    {
        taskContext = serviceProvider.GetRequiredService<ITaskContext>();
        metadataService = serviceProvider.GetRequiredService<IMetadataService>();
        hutaoCache = serviceProvider.GetRequiredService<IHutaoCache>();
        this.serviceProvider = serviceProvider;

        CultivateCommand = new AsyncRelayCommand<Avatar>(CultivateAsync);
        FilterCommand = new RelayCommand<string>(ApplyFilter);
    }

    /// <summary>
    /// 角色列表
    /// </summary>
    public AdvancedCollectionView? Avatars { get => avatars; set => SetProperty(ref avatars, value); }

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
            levelAvatarCurveMap = await metadataService.GetLevelToAvatarCurveMapAsync().ConfigureAwait(false);
            promotes = await metadataService.GetAvatarPromotesAsync().ConfigureAwait(false);

            Dictionary<MaterialId, Material> idMaterialMap = await metadataService.GetIdToMaterialMapAsync().ConfigureAwait(false);
            List<Avatar> avatars = await metadataService.GetAvatarsAsync().ConfigureAwait(false);
            List<Avatar> sorted = avatars
                .OrderByDescending(avatar => avatar.BeginTime)
                .ThenByDescending(avatar => avatar.Sort)
                .ToList();

            await CombineComplexDataAsync(sorted, idMaterialMap).ConfigureAwait(false);

            await taskContext.SwitchToMainThreadAsync();
            Avatars = new AdvancedCollectionView(sorted, true);
            Selected = Avatars.Cast<Avatar>().FirstOrDefault();
        }
    }

    private async Task CombineComplexDataAsync(List<Avatar> avatars, Dictionary<MaterialId, Material> idMaterialMap)
    {
        if (await hutaoCache.InitializeForWikiAvatarViewModelAsync().ConfigureAwait(false))
        {
            Dictionary<AvatarId, AvatarCollocationView> idCollocations = hutaoCache.AvatarCollocations!.ToDictionary(a => a.AvatarId);

            foreach (Avatar avatar in avatars)
            {
                avatar.Collocation = idCollocations.GetValueOrDefault(avatar.Id);
                avatar.CookBonusView ??= CookBonusView.Create(avatar.FetterInfo.CookBonus, idMaterialMap);
                avatar.CultivationItemsView ??= avatar.CultivationItems.SelectList(i => idMaterialMap[i]);
            }
        }
    }

    private async Task CultivateAsync(Avatar? avatar)
    {
        if (avatar != null)
        {
            IInfoBarService infoBarService = serviceProvider.GetRequiredService<IInfoBarService>();
            IUserService userService = serviceProvider.GetRequiredService<IUserService>();

            if (userService.Current != null)
            {
                // ContentDialog must be created by main thread.
                await taskContext.SwitchToMainThreadAsync();
                CultivatePromotionDeltaDialog dialog = serviceProvider.CreateInstance<CultivatePromotionDeltaDialog>(avatar.ToCalculable());
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

                        List<CalcItem> items = CalcItemHelper.Merge(consumption.AvatarConsume, consumption.AvatarSkillConsume);
                        try
                        {
                            bool saved = await serviceProvider
                                .GetRequiredService<ICultivationService>()
                                .SaveConsumptionAsync(CultivateType.AvatarAndSkill, avatar.Id, items)
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

    private void UpdateBaseValueInfo(Avatar? avatar)
    {
        if (avatar == null)
        {
            BaseValueInfo = null;
        }
        else
        {
            Dictionary<int, Promote> avatarPromoteMap = promotes!.Where(p => p.Id == avatar.PromoteId).ToDictionary(p => p.Level);

            FightProperty promoteProperty = avatarPromoteMap[0].AddProperties.Keys.Last();

            List<PropertyCurveValue> propertyCurveValues = new()
            {
                new(FightProperty.FIGHT_PROP_BASE_HP, avatar.GrowCurves[FightProperty.FIGHT_PROP_BASE_HP], avatar.BaseValue.HpBase),
                new(FightProperty.FIGHT_PROP_BASE_ATTACK, avatar.GrowCurves[FightProperty.FIGHT_PROP_BASE_ATTACK], avatar.BaseValue.AttackBase),
                new(FightProperty.FIGHT_PROP_BASE_DEFENSE, avatar.GrowCurves[FightProperty.FIGHT_PROP_BASE_DEFENSE], avatar.BaseValue.DefenseBase),
                new(promoteProperty, GrowCurveType.GROW_CURVE_NONE, 0),
            };

            BaseValueInfo = new(avatar.MaxLevel, propertyCurveValues, levelAvatarCurveMap!, avatarPromoteMap);
        }
    }

    private void ApplyFilter(string? input)
    {
        if (Avatars != null)
        {
            if (!string.IsNullOrWhiteSpace(input))
            {
                Avatars.Filter = AvatarFilter.Compile(input);

                if (!Avatars.Contains(Selected))
                {
                    try
                    {
                        Avatars.MoveCurrentToFirst();
                    }
                    catch (COMException)
                    {
                    }
                }
            }
            else
            {
                Avatars.Filter = null!;
            }
        }
    }

    private static class AvatarFilter
    {
        public static Predicate<object> Compile(string input)
        {
            return (object o) => o is Avatar avatar && DoFilter(input, avatar);
        }

        private static bool DoFilter(string input, Avatar avatar)
        {
            List<bool> matches = new();

            foreach (StringSegment segment in new StringTokenizer(input, ' '.Enumerate().ToArray()))
            {
                string value = segment.ToString();

                if (avatar.Name == value)
                {
                    matches.Add(true);
                    continue;
                }

                if (IntrinsicImmutables.ElementNames.Contains(value))
                {
                    matches.Add(avatar.FetterInfo.VisionBefore == value);
                    continue;
                }

                if (IntrinsicImmutables.AssociationTypes.Contains(value))
                {
                    matches.Add(avatar.FetterInfo.Association.GetLocalizedDescriptionOrDefault() == value);
                    continue;
                }

                if (IntrinsicImmutables.WeaponTypes.Contains(value))
                {
                    matches.Add(avatar.Weapon.GetLocalizedDescriptionOrDefault() == value);
                    continue;
                }

                if (IntrinsicImmutables.ItemQualities.Contains(value))
                {
                    matches.Add(avatar.Quality.GetLocalizedDescriptionOrDefault() == value);
                    continue;
                }

                if (IntrinsicImmutables.BodyTypes.Contains(value))
                {
                    matches.Add(avatar.Body.GetLocalizedDescriptionOrDefault() == value);
                    continue;
                }

                matches.Add(false);
            }

            return matches.Count > 0 && matches.Aggregate((a, b) => a && b);
        }
    }
}