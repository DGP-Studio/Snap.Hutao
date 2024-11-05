// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.Service.Cultivation.Consumption;
using Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate;

namespace Snap.Hutao.UI.Xaml.View.Dialog;

[DependencyProperty("PromotionDelta", typeof(AvatarPromotionDelta))]
internal sealed partial class CultivatePromotionDeltaBatchDialog : ContentDialog
{
    private readonly ITaskContext taskContext;

    public CultivatePromotionDeltaBatchDialog(IServiceProvider serviceProvider)
    {
        InitializeComponent();

        taskContext = serviceProvider.GetRequiredService<ITaskContext>();

        PromotionDelta = AvatarPromotionDelta.CreateForBaseline();
    }

    public async ValueTask<ValueResult<bool, CultivatePromotionDeltaOptions>> GetPromotionDeltaBaselineAsync()
    {
        await taskContext.SwitchToMainThreadAsync();
        ContentDialogResult result = await ShowAsync();

        if (result is not ContentDialogResult.Primary)
        {
            return new(false, default!);
        }

        LocalSetting.Set(SettingKeys.CultivationAvatarLevelCurrent, PromotionDelta.AvatarLevelCurrent);
        LocalSetting.Set(SettingKeys.CultivationAvatarLevelTarget, PromotionDelta.AvatarLevelTarget);

        if (PromotionDelta.SkillList is [{ } skillA, { } skillE, { } skillQ, ..])
        {
            LocalSetting.Set(SettingKeys.CultivationAvatarSkillACurrent, skillA.LevelCurrent);
            LocalSetting.Set(SettingKeys.CultivationAvatarSkillATarget, skillA.LevelTarget);
            LocalSetting.Set(SettingKeys.CultivationAvatarSkillECurrent, skillE.LevelCurrent);
            LocalSetting.Set(SettingKeys.CultivationAvatarSkillETarget, skillE.LevelTarget);
            LocalSetting.Set(SettingKeys.CultivationAvatarSkillQCurrent, skillQ.LevelCurrent);
            LocalSetting.Set(SettingKeys.CultivationAvatarSkillQTarget, skillQ.LevelTarget);
        }

        if (PromotionDelta.Weapon is { } weapon)
        {
            LocalSetting.Set(SettingKeys.CultivationWeapon90LevelCurrent, weapon.LevelCurrent);
            LocalSetting.Set(SettingKeys.CultivationWeapon90LevelTarget, weapon.LevelTarget);
        }

        return new(true, new(PromotionDelta, (ConsumptionSaveStrategyKind)SaveModeSelector.SelectedIndex));
    }
}