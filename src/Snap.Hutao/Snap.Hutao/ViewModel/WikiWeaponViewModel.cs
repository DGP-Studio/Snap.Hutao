// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.WinUI.UI;
using Microsoft.Extensions.Primitives;
using Snap.Hutao.Extension;
using Snap.Hutao.Factory.Abstraction;
using Snap.Hutao.Model.Binding.Cultivation;
using Snap.Hutao.Model.Binding.Hutao;
using Snap.Hutao.Model.Intrinsic;
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
using CalcAvatarPromotionDelta = Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate.AvatarPromotionDelta;
using CalcClient = Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate.CalculateClient;
using CalcConsumption = Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate.Consumption;

namespace Snap.Hutao.ViewModel;

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

    private readonly IMetadataService metadataService;
    private readonly IHutaoCache hutaoCache;

    private AdvancedCollectionView? weapons;
    private Weapon? selected;
    private string? filterText;

    /// <summary>
    /// 构造一个新的武器资料视图模型
    /// </summary>
    /// <param name="metadataService">元数据服务</param>
    /// <param name="hutaoCache">胡桃缓存</param>
    /// <param name="asyncRelayCommandFactory">异步命令工厂</param>
    public WikiWeaponViewModel(IMetadataService metadataService, IHutaoCache hutaoCache, IAsyncRelayCommandFactory asyncRelayCommandFactory)
    {
        this.metadataService = metadataService;
        this.hutaoCache = hutaoCache;

        OpenUICommand = asyncRelayCommandFactory.Create(OpenUIAsync);
        CultivateCommand = asyncRelayCommandFactory.Create<Weapon>(CultivateAsync);
        FilterCommand = new RelayCommand<string>(ApplyFilter);
    }

    /// <summary>
    /// 角色列表
    /// </summary>
    public AdvancedCollectionView? Weapons { get => weapons; set => SetProperty(ref weapons, value); }

    /// <summary>
    /// 选中的角色
    /// </summary>
    public Weapon? Selected { get => selected; set => SetProperty(ref selected, value); }

    /// <summary>
    /// 筛选文本
    /// </summary>
    public string? FilterText { get => filterText; set => SetProperty(ref filterText, value); }

    /// <summary>
    /// 打开界面命令
    /// </summary>
    public ICommand OpenUICommand { get; }

    /// <summary>
    /// 养成命令
    /// </summary>
    public ICommand CultivateCommand { get; }

    /// <summary>
    /// 筛选命令
    /// </summary>
    public ICommand FilterCommand { get; }

    private async Task OpenUIAsync()
    {
        if (await metadataService.InitializeAsync().ConfigureAwait(false))
        {
            List<Weapon> weapons = await metadataService.GetWeaponsAsync().ConfigureAwait(false);
            List<Weapon> sorted = weapons
                .Where(weapon => !skippedWeapons.Contains(weapon.Id))
                .OrderByDescending(weapon => weapon.RankLevel)
                .ThenBy(weapon => weapon.WeaponType)
                .ToList();

            await CombineWithWeaponCollocationsAsync(sorted).ConfigureAwait(false);

            await ThreadHelper.SwitchToMainThreadAsync();

            Weapons = new AdvancedCollectionView(sorted, true);
            Selected = Weapons.Cast<Weapon>().FirstOrDefault();
        }
    }

    private async Task CombineWithWeaponCollocationsAsync(List<Weapon> weapons)
    {
        if (await hutaoCache.InitializeForWikiWeaponViewModelAsync().ConfigureAwait(false))
        {
            Dictionary<WeaponId, ComplexWeaponCollocation> idCollocations = hutaoCache.WeaponCollocations!.ToDictionary(a => a.WeaponId);
            weapons.ForEach(w => w.Collocation = idCollocations.GetValueOrDefault(w.Id));
        }
    }

    private async Task CultivateAsync(Weapon? weapon)
    {
        if (weapon != null)
        {
            IInfoBarService infoBarService = Ioc.Default.GetRequiredService<IInfoBarService>();
            IUserService userService = Ioc.Default.GetRequiredService<IUserService>();

            if (userService.Current != null)
            {
                // ContentDialog must be created by main thread.
                await ThreadHelper.SwitchToMainThreadAsync();
                (bool isOk, CalcAvatarPromotionDelta delta) = await new CultivatePromotionDeltaDialog(null, weapon.ToCalculable())
                    .GetPromotionDeltaAsync()
                    .ConfigureAwait(false);

                if (isOk)
                {
                    Response<CalcConsumption> consumptionResponse = await Ioc.Default
                        .GetRequiredService<CalcClient>()
                        .ComputeAsync(userService.Current.Entity, delta)
                        .ConfigureAwait(false);

                    if (consumptionResponse.IsOk())
                    {
                        CalcConsumption consumption = consumptionResponse.Data;

                        bool saved = await Ioc.Default
                            .GetRequiredService<ICultivationService>()
                            .SaveConsumptionAsync(CultivateType.Weapon, weapon.Id, consumption.WeaponConsume.EmptyIfNull())
                            .ConfigureAwait(false);

                        if (saved)
                        {
                            infoBarService.Success("已成功添加至当前养成计划");
                        }
                        else
                        {
                            infoBarService.Warning("请先前往养成计划页面创建计划并选中");
                        }
                    }
                }
            }
            else
            {
                infoBarService.Warning("必须先选择一个用户与角色");
            }
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
                    Weapons.MoveCurrentToFirst();
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
            bool keep = false;

            foreach (StringSegment segment in new StringTokenizer(input, ' '.Enumerate().ToArray()))
            {
                string value = segment.ToString();

                if (ImmutableIntrinsics.WeaponTypes.Contains(value))
                {
                    keep = keep || weapon.WeaponType.GetDescriptionOrNull() == value;
                    continue;
                }

                if (ImmutableIntrinsics.ItemQualities.Contains(value))
                {
                    keep = keep || weapon.Quality.GetDescriptionOrNull() == value;
                    continue;
                }

                if (ImmutableIntrinsics.FightProperties.Contains(value))
                {
                    keep = keep || weapon.Property.Properties.ElementAtOrDefault(1).GetDescriptionOrNull() == value;
                    continue;
                }

                if (weapon.Name == value)
                {
                    keep = true;
                }
            }

            return keep;
        }
    }
}