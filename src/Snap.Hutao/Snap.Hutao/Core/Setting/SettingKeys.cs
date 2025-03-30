// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.Setting;

internal static class SettingKeys
{
    #region MainWindow
    public const string WindowRect = "WindowRect";
    public const string WindowScale = "WindowScale";
    public const string GuideWindowRect = "GuideWindowRect";
    public const string GuideWindowScale = "GuideWindowScale";
    public const string CompactWebView2WindowRect = "CompactWebView2WindowRect";
    public const string CompactWebView2WindowScale = "CompactWebView2WindowScale";
    public const string IsNavPaneOpen = "IsNavPaneOpen";
    public const string IsInfoBarToggleChecked = "IsInfoBarToggleChecked";
    #endregion

    #region Infrastructure
    public const string ExcludedAnnouncementIds = "ExcludedAnnouncementIds";
    #endregion

    #region Application
    public const string LastVersion = "LastVersion";
    public const string LaunchTimes = "LaunchTimes";
    public const string DataFolderPath = "DataFolderPath";
    public const string PreviousDataFolderToDelete = "PreviousDataFolderToDelete";
    public const string GuideState = "Major1Minor13Revision1GuideState";
    public const string StaticResourceImageQuality = "StaticResourceImageQuality";
    public const string StaticResourceImageArchive = "StaticResourceImageArchive";
    public const string HotKeyMouseClickRepeatForever = "HotKeyMouseClickRepeatForever";
    public const string HotKeyKeyPressRepeatForever = "HotKeyKeyPressRepeatForever";
    public const string IsAllocConsoleDebugModeEnabled = "IsAllocConsoleDebugModeEnabled3";
    #endregion

    #region Overlay
    public const string OverlaySelectedCatalogId = "OverlaySelectedCatalogId";
    public const string OverlayWindowIsVisible = "OverlayWindowIsVisible";
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

    public const string ResinStatisticsSelectedDropDistribution = "ResinStatisticsSelectedDropDistribution";
    #endregion

    #region HomeCard Dashboard
    public const string IsHomeCardLaunchGamePresented = "IsHomeCardLaunchGamePresented";
    public const string IsHomeCardGachaStatisticsPresented = "IsHomeCardGachaStatisticsPresented";
    public const string IsHomeCardAchievementPresented = "IsHomeCardAchievementPresented";
    public const string IsHomeCardDailyNotePresented = "IsHomeCardDailyNotePresented";
    public const string IsHomeCardCalendarPresented = "IsHomeCardCalendarPresented";
    public const string IsHomeCardSignInPresented = "IsHomeCardSignInPresented";
    #endregion

    #region Compact WebView2
    public const string LowLevelKeyboardWebView2VideoPlayPause = "LowLevelKeyboardWebView2VideoPlayPause";
    public const string LowLevelKeyboardWebView2VideoFastForward = "LowLevelKeyboardWebView2VideoFastForward";
    public const string LowLevelKeyboardWebView2VideoRewind = "LowLevelKeyboardWebView2VideoRewind";
    public const string LowLevelKeyboardWebView2Hide = "LowLevelKeyboardWebView2Hide";
    public const string LowLevelKeyboardOverlayHide = "LowLevelKeyboardOverlayHide";
    public const string WebView2VideoFastForwardOrRewindSeconds = "WebView2VideoFastForwardOrRewindSeconds";
    public const string CompactWebView2WindowInactiveOpacity = "CompactWebView2WindowInactiveOpacity";
    public const string CompactWebView2WindowPreviousSourceUrl = "CompactWebView2WindowPreviousSourceUrl";
    #endregion

    #region DevTool
    public const string SuppressMetadataInitialization = "SuppressMetadataInitialization";
    public const string OverrideElevationRequirement = "OverrideElevationRequirement";
    public const string OverrideUpdateVersionComparison = "OverrideUpdateVersionComparison";
    public const string OverridePackageConvertDirectoryPermissionsRequirement = "OverridePackageConvertDirectoryPermissionsRequirement";
    public const string OverridePhysicalDriverType = "OverridePhysicalDriverType";
    public const string PhysicalDriverIsAlwaysSolidState = "PhysicalDriverIsAlwaysSolidState";
    public const string AlwaysIsFirstRunAfterUpdate = "AlwaysIsFirstRunAfterUpdate";
    public const string AlphaBuildUseCnPatchEndpoint = "AlphaBuildUseCNPatchEndpoint";
    public const string AlphaBuildUseFjPatchEndpoint = "AlphaBuildUseFJPatchEndpoint";
    #endregion

    #region Obsolete

    [Obsolete("第一次重置新手引导状态")]
    public const string Major1Minor7Revision0GuideState = "Major1Minor7Revision0GuideState";

    [Obsolete("第二次重置新手引导状态")]
    public const string Major1Minor10Revision0GuideState = "Major1Minor10Revision0GuideState1";

    [Obsolete("重置调试控制台开关")]
    public const string IsAllocConsoleDebugModeEnabledLegacy1 = "IsAllocConsoleDebugModeEnabled";

    [Obsolete("重置调试控制台开关2")]
    public const string IsAllocConsoleDebugModeEnabledLegacy2 = "IsAllocConsoleDebugModeEnabled2";

    [Obsolete]
    public const string AllowExtractGameBlks = "AllowExtractGameBlks";

    #endregion
}