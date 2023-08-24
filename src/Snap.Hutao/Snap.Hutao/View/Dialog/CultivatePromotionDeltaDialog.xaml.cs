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
        XamlRoot = serviceProvider.GetRequiredService<MainWindow>().Content.XamlRoot;

        taskContext = serviceProvider.GetRequiredService<ITaskContext>();

        Avatar = options.Avatar;
        Weapon = options.Weapon;
        DataContext = this;
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
            AvatarLevelCurrent = Avatar?.LevelCurrent ?? 0,
            AvatarLevelTarget = Avatar?.LevelTarget ?? 0,
            SkillList = Avatar?.Skills.SelectList(s => new PromotionDelta()
            {
                Id = s.GroupId,
                LevelCurrent = s.LevelCurrent,
                LevelTarget = s.LevelTarget,
            }),
            Weapon = Weapon is null ? null : new PromotionDelta()
            {
                Id = Weapon.WeaponId,
                LevelCurrent = Weapon.LevelCurrent,
                LevelTarget = Weapon.LevelTarget,
            },
        };

        return new(true, delta);
    }
}
