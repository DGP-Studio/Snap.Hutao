// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.WinUI.UI;
using Snap.Hutao.Factory.Abstraction;
using Snap.Hutao.Model;
using Snap.Hutao.Model.Binding.Cultivation;
using Snap.Hutao.Model.Binding.Hutao;
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Metadata.Avatar;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.Service.Abstraction;
using Snap.Hutao.Service.Cultivation;
using Snap.Hutao.Service.Hutao;
using Snap.Hutao.Service.Metadata;
using Snap.Hutao.Service.User;
using Snap.Hutao.View.Dialog;
using CalcAvatarPromotionDelta = Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate.AvatarPromotionDelta;
using CalcClient = Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate.CalculateClient;
using CalcConsumption = Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate.Consumption;
using CalcItem = Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate.Item;
using CalcItemHelper = Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate.ItemHelper;

namespace Snap.Hutao.ViewModel;

/// <summary>
/// 角色资料视图模型
/// </summary>
[Injection(InjectAs.Scoped)]
internal class WikiAvatarViewModel : ObservableObject
{
    private readonly IMetadataService metadataService;
    private readonly IHutaoCache hutaoCache;

    // filters
    private readonly List<Selectable<string>> filterElementInfos;
    private readonly List<Selectable<Pair<string, AssociationType>>> filterAssociationInfos;
    private readonly List<Selectable<Pair<string, WeaponType>>> filterWeaponTypeInfos;
    private readonly List<Selectable<Pair<string, ItemQuality>>> filterQualityInfos;
    private readonly List<Selectable<Pair<string, BodyType>>> filterBodyInfos;

    private AdvancedCollectionView? avatars;
    private Avatar? selected;

    /// <summary>
    /// 构造一个新的角色资料视图模型
    /// </summary>
    /// <param name="metadataService">元数据服务</param>
    /// <param name="hutaoCache">胡桃缓存</param>
    /// <param name="asyncRelayCommandFactory">异步命令工厂</param>
    public WikiAvatarViewModel(IMetadataService metadataService, IHutaoCache hutaoCache, IAsyncRelayCommandFactory asyncRelayCommandFactory)
    {
        this.metadataService = metadataService;
        this.hutaoCache = hutaoCache;
        OpenUICommand = asyncRelayCommandFactory.Create(OpenUIAsync);
        CultivateCommand = asyncRelayCommandFactory.Create<Avatar>(CultivateAsync);

        filterElementInfos = new()
        {
            new("火", OnFilterChanged),
            new("水", OnFilterChanged),
            new("草", OnFilterChanged),
            new("雷", OnFilterChanged),
            new("冰", OnFilterChanged),
            new("风", OnFilterChanged),
            new("岩", OnFilterChanged),
        };

        filterAssociationInfos = new()
        {
            new(new("蒙德", AssociationType.ASSOC_TYPE_MONDSTADT), OnFilterChanged),
            new(new("璃月", AssociationType.ASSOC_TYPE_LIYUE), OnFilterChanged),
            new(new("稻妻", AssociationType.ASSOC_TYPE_INAZUMA), OnFilterChanged),
            new(new("须弥", AssociationType.ASSOC_TYPE_SUMERU), OnFilterChanged),
            new(new("愚人众", AssociationType.ASSOC_TYPE_FATUI), OnFilterChanged),
            new(new("游侠", AssociationType.ASSOC_TYPE_RANGER), OnFilterChanged),
        };

        filterWeaponTypeInfos = new()
        {
            new(new("单手剑", WeaponType.WEAPON_SWORD_ONE_HAND), OnFilterChanged),
            new(new("法器", WeaponType.WEAPON_CATALYST), OnFilterChanged),
            new(new("双手剑", WeaponType.WEAPON_CLAYMORE), OnFilterChanged),
            new(new("弓", WeaponType.WEAPON_BOW), OnFilterChanged),
            new(new("长柄武器", WeaponType.WEAPON_POLE), OnFilterChanged),
        };

        filterQualityInfos = new()
        {
            new(new("限定五星", ItemQuality.QUALITY_ORANGE_SP), OnFilterChanged),
            new(new("五星", ItemQuality.QUALITY_ORANGE), OnFilterChanged),
            new(new("四星", ItemQuality.QUALITY_PURPLE), OnFilterChanged),
        };

        filterBodyInfos = new()
        {
            new(new("成女", BodyType.BODY_LADY), OnFilterChanged),
            new(new("少女", BodyType.BODY_GIRL), OnFilterChanged),
            new(new("幼女", BodyType.BODY_LOLI), OnFilterChanged),
            new(new("成男", BodyType.BODY_MALE), OnFilterChanged),
            new(new("少男", BodyType.BODY_BOY), OnFilterChanged),
        };
    }

    /// <summary>
    /// 角色列表
    /// </summary>
    public AdvancedCollectionView? Avatars { get => avatars; set => SetProperty(ref avatars, value); }

    /// <summary>
    /// 选中的角色
    /// </summary>
    public Avatar? Selected { get => selected; set => SetProperty(ref selected, value); }

    /// <summary>
    /// 筛选用元素信息集合
    /// </summary>
    public IList<Selectable<string>> FilterElementInfos
    {
        get => filterElementInfos;
    }

    /// <summary>
    /// 筛选用所属国家集合
    /// </summary>
    public IList<Selectable<Pair<string, AssociationType>>> FilterAssociationInfos
    {
        get => filterAssociationInfos;
    }

    /// <summary>
    /// 筛选用武器信息集合
    /// </summary>
    public IList<Selectable<Pair<string, WeaponType>>> FilterWeaponTypeInfos
    {
        get => filterWeaponTypeInfos;
    }

    /// <summary>
    /// 筛选用星级信息集合
    /// </summary>
    public IList<Selectable<Pair<string, ItemQuality>>> FilterQualityInfos
    {
        get => filterQualityInfos;
    }

    /// <summary>
    /// 筛选用体型信息集合
    /// </summary>
    public IList<Selectable<Pair<string, BodyType>>> FilterBodyInfos
    {
        get => filterBodyInfos;
    }

    /// <summary>
    /// 打开页面命令
    /// </summary>
    public ICommand OpenUICommand { get; }

    /// <summary>
    /// 养成命令
    /// </summary>
    public ICommand CultivateCommand { get; }

    private async Task OpenUIAsync()
    {
        if (await metadataService.InitializeAsync().ConfigureAwait(false))
        {
            List<Avatar> avatars = await metadataService.GetAvatarsAsync().ConfigureAwait(false);
            List<Avatar> sorted = avatars
                .OrderByDescending(avatar => avatar.BeginTime)
                .ThenByDescending(avatar => avatar.Sort)
                .ToList();

            await CombineWithAvatarCollocationsAsync(sorted).ConfigureAwait(false);

            await ThreadHelper.SwitchToMainThreadAsync();
            Avatars = new AdvancedCollectionView(sorted, true);
            Selected = Avatars.Cast<Avatar>().FirstOrDefault();
        }
    }

    private async Task CombineWithAvatarCollocationsAsync(List<Avatar> avatars)
    {
        if (await hutaoCache.InitializeForWikiAvatarViewModelAsync().ConfigureAwait(false))
        {
            Dictionary<AvatarId, ComplexAvatarCollocation> idCollocations = hutaoCache.AvatarCollocations!.ToDictionary(a => a.AvatarId);

            foreach (Avatar avatar in avatars)
            {
                avatar.Collocation = idCollocations.GetValueOrDefault(avatar.Id);
            }
        }
    }

    private void OnFilterChanged()
    {
        if (Avatars is not null)
        {
            List<string> targetElements = filterElementInfos
                .Where(e => e.IsSelected)
                .Select(e => e.Value)
                .ToList();

            List<AssociationType> targetAssociations = filterAssociationInfos
                .Where(e => e.IsSelected)
                .Select(e => e.Value.Value)
                .ToList();

            List<WeaponType> targetWeaponTypes = filterWeaponTypeInfos
                .Where(e => e.IsSelected)
                .Select(e => e.Value.Value)
                .ToList();

            List<ItemQuality> targetQualities = FilterQualityInfos
                .Where(e => e.IsSelected)
                .Select(e => e.Value.Value)
                .ToList();

            List<BodyType> targetBodies = filterBodyInfos
                .Where(e => e.IsSelected)
                .Select(e => e.Value.Value)
                .ToList();

            Avatars.Filter = (object o) => o is Avatar avatar
                && targetElements.Contains(avatar.FetterInfo.VisionBefore)
                && targetAssociations.Contains(avatar.FetterInfo.Association)
                && targetWeaponTypes.Contains(avatar.Weapon)
                && targetQualities.Contains(avatar.Quality)
                && targetBodies.Contains(avatar.Body);

            if (!Avatars.Contains(Selected))
            {
                Avatars.MoveCurrentToFirst();
            }
        }
    }

    private async Task CultivateAsync(Avatar? avatar)
    {
        IInfoBarService infoBarService = Ioc.Default.GetRequiredService<IInfoBarService>();
        if (avatar != null)
        {
            IUserService userService = Ioc.Default.GetRequiredService<IUserService>();
            if (userService.Current != null)
            {
                MainWindow mainWindow = Ioc.Default.GetRequiredService<MainWindow>();
                (bool isOk, CalcAvatarPromotionDelta delta) = await new CultivatePromotionDeltaDialog(mainWindow, avatar.ToCalculable(), null)
                    .GetPromotionDeltaAsync()
                    .ConfigureAwait(false);

                if (isOk)
                {
                    CalcClient calculateClient = Ioc.Default.GetRequiredService<CalcClient>();
                    CalcConsumption? consumption = await calculateClient.ComputeAsync(userService.Current.Entity, delta).ConfigureAwait(false);
                    if (consumption != null)
                    {
                        List<CalcItem> items = CalcItemHelper.Merge(consumption.AvatarConsume, consumption.AvatarSkillConsume);
                        bool saved = await Ioc.Default
                            .GetRequiredService<ICultivationService>()
                            .SaveConsumptionAsync(CultivateType.AvatarAndSkill, avatar.Id, items)
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
}