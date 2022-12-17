// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Control;
using Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate;

namespace Snap.Hutao.View.Dialog;

/// <summary>
/// 养成计算对话框
/// </summary>
public sealed partial class CultivatePromotionDeltaDialog : ContentDialog
{
    private static readonly DependencyProperty AvatarProperty = Property<CultivatePromotionDeltaDialog>.Depend<ICalculableAvatar?>(nameof(Avatar));
    private static readonly DependencyProperty WeaponProperty = Property<CultivatePromotionDeltaDialog>.Depend<ICalculableWeapon?>(nameof(Weapon));

    /// <summary>
    /// 构造一个新的养成计算对话框
    /// </summary>
    /// <param name="window">窗体</param>
    /// <param name="avatar">角色</param>
    /// <param name="weapon">武器</param>
    public CultivatePromotionDeltaDialog(Window window, ICalculableAvatar? avatar, ICalculableWeapon? weapon)
    {
        InitializeComponent();
        XamlRoot = window.Content.XamlRoot;
        DataContext = this;
        Avatar = avatar;
        Weapon = weapon;
    }

    /// <summary>
    /// 角色
    /// </summary>
    public ICalculableAvatar? Avatar
    {
        get { return (ICalculableAvatar?)GetValue(AvatarProperty); }
        set { SetValue(AvatarProperty, value); }
    }

    /// <summary>
    /// 武器
    /// </summary>
    public ICalculableWeapon? Weapon
    {
        get { return (ICalculableWeapon?)GetValue(WeaponProperty); }
        set { SetValue(WeaponProperty, value); }
    }

    /// <summary>
    /// 异步获取提升差异
    /// </summary>
    /// <returns>提升差异</returns>
    public async Task<ValueResult<bool, AvatarPromotionDelta>> GetPromotionDeltaAsync()
    {
        ContentDialogResult result = await ShowAsync();

        if (result == ContentDialogResult.Primary)
        {
            AvatarPromotionDelta delta = new()
            {
                AvatarId = Avatar?.AvatarId ?? 0,
                AvatarLevelCurrent = Avatar?.LevelCurrent ?? 0,
                AvatarLevelTarget = Avatar?.LevelTarget ?? 0,
                SkillList = Avatar?.Skills.Select(s => new PromotionDelta()
                {
                    Id = s.GruopId,
                    LevelCurrent = s.LevelCurrent,
                    LevelTarget = s.LevelTarget,
                }),
                Weapon = Weapon == null ? null : new PromotionDelta()
                {
                    Id = Weapon.WeaponId,
                    LevelCurrent = Weapon.LevelCurrent,
                    LevelTarget = Weapon.LevelTarget,
                },
            };

            return new(true, delta);
        }
        else
        {
            return new(false, null!);
        }
    }
}
