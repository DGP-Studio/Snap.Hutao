// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Model.Calculable;
using Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate;

namespace Snap.Hutao.View.Dialog;

/// <summary>
/// 养成计算对话框
/// </summary>
[HighQuality]
[DependencyProperty("Avatar", typeof(ICalculableAvatar))]
[DependencyProperty("Weapon", typeof(ICalculableWeapon))]
internal sealed partial class CultivatePromotionDeltaDialog : ContentDialog
{
    private readonly ITaskContext taskContext;

    /// <summary>
    /// 构造一个新的养成计算对话框
    /// </summary>
    /// <param name="serviceProvider">服务提供器</param>
    /// <param name="options">选项</param>
    [SuppressMessage("", "SH002")]
    public CultivatePromotionDeltaDialog(IServiceProvider serviceProvider, CalculableOptions options)
    {
        InitializeComponent();

        taskContext = serviceProvider.GetRequiredService<ITaskContext>();

        Avatar = options.Avatar;
        Weapon = options.Weapon;
    }

    /// <summary>
    /// 异步获取提升差异
    /// </summary>
    /// <returns>提升差异</returns>
    public async ValueTask<ValueResult<bool, AvatarPromotionDelta>> GetPromotionDeltaAsync()
    {
        await taskContext.SwitchToMainThreadAsync();
        ContentDialogResult result = await ShowAsync();

        if (result != ContentDialogResult.Primary)
        {
            return new(false, default!);
        }

        AvatarPromotionDelta delta = new()
        {
            AvatarId = Avatar?.AvatarId ?? 0,
            AvatarLevelCurrent = Avatar is not null ? Math.Clamp(Avatar.LevelCurrent, Avatar.LevelMin, Avatar.LevelMax) : 0,
            AvatarLevelTarget = Avatar is not null ? Math.Clamp(Avatar.LevelTarget, Avatar.LevelMin, Avatar.LevelMax) : 0,
            SkillList = Avatar?.Skills.SelectList(skill => new PromotionDelta()
            {
                Id = skill.GroupId,
                LevelCurrent = Math.Clamp(skill.LevelCurrent, skill.LevelMin, skill.LevelMax),
                LevelTarget = Math.Clamp(skill.LevelTarget, skill.LevelMin, skill.LevelMax),
            }),
            Weapon = Weapon is null ? null : new PromotionDelta()
            {
                Id = Weapon.WeaponId,
                LevelCurrent = Math.Clamp(Weapon.LevelCurrent, Weapon.LevelMin, Weapon.LevelMax),
                LevelTarget = Math.Clamp(Weapon.LevelTarget, Weapon.LevelMin, Weapon.LevelMax),
            },
        };

        return new(true, delta);
    }
}
