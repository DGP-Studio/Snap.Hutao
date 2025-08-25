// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.Model.Calculable;
using Snap.Hutao.Service.Cultivation.Consumption;
using Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate;

namespace Snap.Hutao.UI.Xaml.View.Dialog;

[DependencyProperty<ICalculableAvatar>("Avatar")]
[DependencyProperty<ICalculableWeapon>("Weapon")]
internal sealed partial class CultivatePromotionDeltaDialog : ContentDialog
{
    private readonly IContentDialogFactory contentDialogFactory;

    public CultivatePromotionDeltaDialog(IServiceProvider serviceProvider, CalculableOptions options)
    {
        InitializeComponent();

        contentDialogFactory = serviceProvider.GetRequiredService<IContentDialogFactory>();

        Avatar = options.Avatar;
        Weapon = options.Weapon;
    }

    public async ValueTask<ValueResult<bool, CultivatePromotionDeltaOptions>> GetPromotionDeltaAsync()
    {
        if (await contentDialogFactory.EnqueueAndShowAsync(this).ShowTask.ConfigureAwait(false) is not ContentDialogResult.Primary)
        {
            return new(false, default!);
        }

        await contentDialogFactory.TaskContext.SwitchToMainThreadAsync();

        AvatarPromotionDelta delta = new()
        {
            AvatarId = Avatar?.AvatarId ?? 0,
            AvatarLevelCurrent = Avatar is not null ? Math.Clamp(Avatar.LevelCurrent, Avatar.LevelMin, Avatar.LevelMax) : 0,
            AvatarLevelTarget = Avatar is not null ? Math.Clamp(Avatar.LevelTarget, Avatar.LevelMin, Avatar.LevelMax) : 0,
            AvatarPromoteLevel = Avatar?.PromoteLevel ?? 0,
            SkillList = Avatar?.Skills.SelectAsArray(static skill => new PromotionDelta
            {
                Id = skill.GroupId,
                LevelCurrent = Math.Clamp(skill.LevelCurrent, skill.LevelMin, skill.LevelMax),
                LevelTarget = Math.Clamp(skill.LevelTarget, skill.LevelMin, skill.LevelMax),
            }) ?? default,
            Weapon = Weapon is null ? null : new PromotionDelta
            {
                Id = Weapon.WeaponId,
                LevelCurrent = Math.Clamp(Weapon.LevelCurrent, Weapon.LevelMin, Weapon.LevelMax),
                LevelTarget = Math.Clamp(Weapon.LevelTarget, Weapon.LevelMin, Weapon.LevelMax),
                WeaponPromoteLevel = Weapon?.PromoteLevel ?? 0,
            },
        };

        return new(true, new(delta, (ConsumptionSaveStrategyKind)SaveModeSelector.SelectedIndex));
    }
}