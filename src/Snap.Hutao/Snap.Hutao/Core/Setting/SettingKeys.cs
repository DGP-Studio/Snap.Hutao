// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.Setting;

/// <summary>
/// 设置键
/// </summary>
[HighQuality]
[SuppressMessage("", "SA1124")]
internal static class SettingKeys
{
    #region MainWindow
    public const string WindowRect = "WindowRect";
    public const string IsNavPaneOpen = "IsNavPaneOpen";
    public const string IsInfoBarToggleChecked = "IsInfoBarToggleChecked";
    public const string ExcludedAnnouncementIds = "ExcludedAnnouncementIds";
    #endregion

    #region Application
    public const string LaunchTimes = "LaunchTimes";
    public const string DataFolderPath = "DataFolderPath";
    public const string Major1Minor7Revision0GuideState = "Major1Minor7Revision0GuideState";
    public const string HotKeyMouseClickRepeatForever = "HotKeyMouseClickRepeatForever";
    public const string IsAllocConsoleDebugModeEnabled = "IsAllocConsoleDebugModeEnabled2";
    #endregion

    #region Passport
    public const string PassportUserName = "PassportUserName";
    public const string PassportPassword = "PassportPassword";
    #endregion

    #region Cultivation
    public const string CultivationAvatarLevelCurrent = "CultivationAvatarLevelCurrent";
    public const string CultivationAvatarLevelTarget = "CultivationAvatarLevelTarget";
    public const string CultivationAvatarSkillACurrent = "CultivationAvatarSkillACurrent";
    public const string CultivationAvatarSkillATarget = "CultivationAvatarSkillATarget";
    public const string CultivationAvatarSkillECurrent = "CultivationAvatarSkillECurrent";
    public const string CultivationAvatarSkillETarget = "CultivationAvatarSkillETarget";
    public const string CultivationAvatarSkillQCurrent = "CultivationAvatarSkillQCurrent";
    public const string CultivationAvatarSkillQTarget = "CultivationAvatarSkillQTarget";
    public const string CultivationWeapon90LevelCurrent = "CultivationWeapon90LevelCurrent";
    public const string CultivationWeapon90LevelTarget = "CultivationWeapon90LevelTarget";
    public const string CultivationWeapon70LevelCurrent = "CultivationWeapon70LevelCurrent";
    public const string CultivationWeapon70LevelTarget = "CultivationWeapon70LevelTarget";
    #endregion

    #region HomeCard Dashboard
    public const string IsHomeCardLaunchGamePresented = "IsHomeCardLaunchGamePresented";
    public const string IsHomeCardGachaStatisticsPresented = "IsHomeCardGachaStatisticsPresented";
    public const string IsHomeCardAchievementPresented = "IsHomeCardAchievementPresented";
    public const string IsHomeCardDailyNotePresented = "IsHomeCardDailyNotePresented";
    #endregion

    #region DevTool
    public const string SuppressMetadataInitialization = "SuppressMetadataInitialization";
    public const string OverrideElevationRequirement = "OverrideElevationRequirement";
    public const string OverrideUpdateVersionComparison = "OverrideUpdateVersionComparison";
    #endregion

    #region Obsolete
    [Obsolete("重置调试控制台开关")]
    public const string IsAllocConsoleDebugModeEnabledLegacy1 = "IsAllocConsoleDebugModeEnabled";
    #endregion
}