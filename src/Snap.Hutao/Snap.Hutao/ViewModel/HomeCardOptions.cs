// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Setting;

namespace Snap.Hutao.ViewModel;

internal sealed class HomeCardOptions
{
    public bool IsHomeCardLaunchGamePresented
    {
        get => LocalSetting.Get(SettingKeys.IsHomeCardLaunchGamePresented, true);
        set => LocalSetting.Set(SettingKeys.IsHomeCardLaunchGamePresented, value);
    }

    public bool IsHomeCardGachaStatisticsPresented
    {
        get => LocalSetting.Get(SettingKeys.IsHomeCardGachaStatisticsPresented, true);
        set => LocalSetting.Set(SettingKeys.IsHomeCardGachaStatisticsPresented, value);
    }

    public bool IsHomeCardAchievementPresented
    {
        get => LocalSetting.Get(SettingKeys.IsHomeCardAchievementPresented, true);
        set => LocalSetting.Set(SettingKeys.IsHomeCardAchievementPresented, value);
    }

    public bool IsHomeCardDailyNotePresented
    {
        get => LocalSetting.Get(SettingKeys.IsHomeCardDailyNotePresented, true);
        set => LocalSetting.Set(SettingKeys.IsHomeCardDailyNotePresented, value);
    }
}