// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.WinUI.UI;
using Snap.Hutao.Control;
using Snap.Hutao.Factory.Abstraction;
using Snap.Hutao.Model.Binding.Hutao;
using Snap.Hutao.Model.Metadata.Weapon;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.Service.Hutao;
using Snap.Hutao.Service.Metadata;

namespace Snap.Hutao.ViewModel;

/// <summary>
/// 武器资料视图模型
/// </summary>
[Injection(InjectAs.Scoped)]
internal class WikiWeaponViewModel : ObservableObject, ISupportCancellation
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
    }

    /// <inheritdoc/>
    public CancellationToken CancellationToken { get; set; }

    /// <summary>
    /// 角色列表
    /// </summary>
    public AdvancedCollectionView? Weapons { get => weapons; set => SetProperty(ref weapons, value); }

    /// <summary>
    /// 选中的角色
    /// </summary>
    public Weapon? Selected { get => selected; set => SetProperty(ref selected, value); }

    /// <summary>
    /// 打开界面命令
    /// </summary>
    public ICommand OpenUICommand { get; }

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
}