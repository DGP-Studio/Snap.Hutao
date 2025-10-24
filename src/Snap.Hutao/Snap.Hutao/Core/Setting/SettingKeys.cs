// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.Setting;

internal static class SettingKeys
{
    #region MainWindow
    public const string IsNavPaneOpen = "IsNavPaneOpen";
    #endregion

    #region Infrastructure
    public const string ExcludedAnnouncementIds = "ExcludedAnnouncementIds";
    #endregion

    #region Application
    public const string LastVersion = "LastVersion";
    public const string LaunchTimes = "LaunchTimes";
    public const string DataDirectory = "DataFolderPath";
    public const string PreviousDataDirectoryToDelete = "PreviousDataFolderToDelete";
    public const string GuideState = "Major1Minor13Revision1GuideState";
    public const string StaticResourceImageQuality = "StaticResourceImageQuality";
    public const string StaticResourceImageArchive = "StaticResourceImageArchive";
    public const string HotKeyRepeatForeverInGameOnly = "HotKeyRepeatForeverInGameOnly";
    public const string HotKeyMouseClickRepeatForever = "HotKeyMouseClickRepeatForever2";
    public const string HotKeyKeyPressRepeatForever = "HotKeyKeyPressRepeatForever2";
    public const string IsLastWindowCloseBehaviorSet = "IsLastWindowCloseBehaviorSet";
    #endregion

    #region Launching
    public const string LaunchExecutionArbitraryLibrary = "LaunchExecutionArbitraryLibrary";
    public const string LaunchForceUsingTouchScreenWhenIntegratedTouchPresent = "Launch.ForceUsingTouchScreenWhenIntegratedTouchPresent";
    #endregion

    #region Overlay
    public const string OverlaySelectedCatalogId = "OverlaySelectedCatalogId";
    public const string OverlayWindowIsVisible = "OverlayWindowIsVisible2";
    #endregion

    #region Passport
    public const string PassportUserName = "PassportUserName";
    public const string PassportRefreshToken = "PassportRefreshToken";
    #endregion

    #region AvatarProperty
    public const string AvatarPropertySortDescriptionKind = "AvatarPropertySortDescriptionKind";
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
    public const string HomeCardLaunchGameOrder = "HomeCardLaunchGameOrder";

    public const string IsHomeCardGachaStatisticsPresented = "IsHomeCardGachaStatisticsPresented";
    public const string HomeCardGachaStatisticsOrder = "HomeCardGachaStatisticsOrder";

    public const string IsHomeCardAchievementPresented = "IsHomeCardAchievementPresented";
    public const string HomeCardAchievementOrder = "HomeCardAchievementOrder";

    public const string IsHomeCardDailyNotePresented = "IsHomeCardDailyNotePresented";
    public const string HomeCardDailyNoteOrder = "HomeCardDailyNoteOrder";

    public const string IsHomeCardCalendarPresented = "IsHomeCardCalendarPresented";
    public const string HomeCardCalendarOrder = "HomeCardCalendarOrder";

    public const string IsHomeCardSignInPresented = "IsHomeCardSignInPresented";
    public const string HomeCardSignInOrder = "HomeCardSignInOrder";
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
    public const string TreatPredownloadAsMain = "TreatPredownloadAsMain";
    public const string EnableBetaGameInstall = "EnableBetaGameInstall";
    public const string PreventCopyIslandDll = "PreventCopyIslandDll";
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
    public const string IsAllocConsoleDebugModeEnabledLegacy3 = "IsAllocConsoleDebugModeEnabled3";

    [Obsolete]
    public const string AllowExtractGameBlks = "AllowExtractGameBlks";

    [Obsolete("第一次重置悬浮窗初始状态")]
    public const string OverlayWindowIsVisibleLegacy1 = "OverlayWindowIsVisible";

    [Obsolete]
    public const string HotKeyMouseClickRepeatForeverLegacy1 = "HotKeyMouseClickRepeatForever";

    [Obsolete]
    public const string HotKeyKeyPressRepeatForeverLegacy1 = "HotKeyKeyPressRepeatForever";

    [Obsolete]
    public const string WindowRect = "WindowRect";

    [Obsolete]
    public const string WindowScale = "WindowScale";

    [Obsolete]
    public const string GuideWindowRect = "GuideWindowRect";

    [Obsolete]
    public const string GuideWindowScale = "GuideWindowScale";

    [Obsolete]
    public const string CompactWebView2WindowRect = "CompactWebView2WindowRect";

    [Obsolete]
    public const string CompactWebView2WindowScale = "CompactWebView2WindowScale";

    [Obsolete]
    public const string IsInfoBarToggleChecked = "IsInfoBarToggleChecked";

    [Obsolete("不再保存通行证密码")]
    public const string PassportPassword = "PassportPassword";

    [Obsolete]
    public const string EnableOfflineCultivationCalculator = "EnableOfflineCultivationCalculator";
    #endregion
}