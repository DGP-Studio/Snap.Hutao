// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.Service.Cultivation.Consumption;
using Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate;

namespace Snap.Hutao.UI.Xaml.View.Dialog;

[DependencyProperty<AvatarPromotionDelta>("PromotionDelta", NotNull = true, CreateDefaultValueCallbackName = nameof(CreatePromotionDeltaDefaultValue))]
internal sealed partial class CultivatePromotionDeltaBatchDialog : ContentDialog
{
    private readonly IContentDialogFactory contentDialogFactory;

    [GeneratedConstructor(InitializeComponent = true)]
    public partial CultivatePromotionDeltaBatchDialog(IServiceProvider serviceProvider);

    public async ValueTask<ValueResult<bool, CultivatePromotionDeltaOptions>> GetPromotionDeltaBaselineAsync()
    {
        if (await contentDialogFactory.EnqueueAndShowAsync(this).ShowTask.ConfigureAwait(false) is not ContentDialogResult.Primary)
        {
            return new(false, default!);
        }

        await contentDialogFactory.TaskContext.SwitchToMainThreadAsync();

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

    private static object CreatePromotionDeltaDefaultValue()
    {
        return AvatarPromotionDelta.CreateForBaseline();
    }
}