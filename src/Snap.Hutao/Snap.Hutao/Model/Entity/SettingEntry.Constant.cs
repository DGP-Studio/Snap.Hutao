// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Model.Entity;

/// <summary>
/// 键名
/// </summary>
internal sealed partial class SettingEntry
{
    public const string GamePath = "GamePath";
    public const string GamePathEntries = "GamePathEntries";
    public const string AspectRatios = "AspectRatios";
    public const string Culture = "Culture";
    public const string FirstDayOfWeek = "FirstDayOfWeek";

    public const string LastWindowCloseBehavior = "LastWindowCloseBehavior";
    public const string SystemBackdropType = "SystemBackdropType";
    public const string ElementTheme = "ElementTheme";
    public const string BackgroundImageType = "BackgroundImageType";

    public const string AnnouncementRegion = "AnnouncementRegion";

    public const string IsEmptyHistoryWishVisible = "IsEmptyHistoryWishVisible";
    public const string IsUnobtainedWishItemVisible = "IsUnobtainedWishItemVisible";

    public const string GeetestCustomCompositeUrl = "GeetestCustomCompositeUrl";

    public const string DownloadSpeedLimitPerSecondInKiloByte = "DownloadSpeedLimitPerSecondInKiloByte";

    public const string BridgeShareSaveType = "BridgeShareSaveType";

    public const string CalendarServerTimeZoneOffset = "CalendarServerTimeZoneOffset";

    public const string DailyNoteIsAutoRefreshEnabled = "DailyNote.IsAutoRefreshEnabled";
    public const string DailyNoteRefreshSeconds = "DailyNote.RefreshSeconds";
    public const string DailyNoteReminderNotify = "DailyNote.ReminderNotify";
    public const string DailyNoteSilentWhenPlayingGame = "DailyNote.SilentWhenPlayingGame";
    public const string DailyNoteWebhookUrl = "DailyNote.WebhookUrl";

    public const string LaunchAreCommandLineArgumentsEnabled = "Launch.IsLaunchOptionsEnabled";
    public const string LaunchIsExclusive = "Launch.IsExclusive";
    public const string LaunchIsFullScreen = "Launch.IsFullScreen";
    public const string LaunchIsBorderless = "Launch.IsBorderless";
    public const string LaunchScreenWidth = "Launch.ScreenWidth";
    public const string LaunchIsScreenWidthEnabled = "Launch.IsScreenWidthEnabled";
    public const string LaunchScreenHeight = "Launch.ScreenHeight";
    public const string LaunchIsScreenHeightEnabled = "Launch.IsScreenHeightEnabled";
    public const string LaunchIsIslandEnabled = "Launch.UnlockFps";
    public const string LaunchIsSetFieldOfViewEnabled = "Launch.IsSetFieldOfViewEnabled";
    public const string LaunchTargetFov = "Launch.TargetFov";
    public const string LaunchFixLowFovScene = "Launch.FixLowFovScene";
    public const string LaunchDisableFog = "Launch.DisableFog";
    public const string LaunchIsSetTargetFrameRateEnabled = "Launch.IsSetTargetFrameRateEnabled";
    public const string LaunchTargetFps = "Launch.TargetFps";
    public const string LaunchRemoveOpenTeamProgress = "Launch.RemoveOpenTeamProgress";
    public const string LaunchHideQuestBanner = "Launch.HideQuestBanner";
    public const string LaunchDisableEventCameraMove = "Launch.DisableEventCameraMove";
    public const string LaunchDisableShowDamageText = "Launch.DisableShowDamageText";
    public const string LaunchUsingTouchScreen = "Launch.UsingTouchScreen";
    public const string LaunchRedirectCombineEntry = "Launch.RedirectCombineEntry";
    public const string LaunchMonitor = "Launch.Monitor";
    public const string LaunchIsMonitorEnabled = "Launch.IsMonitorEnabled";
    public const string LaunchPlatformType = "Launch.PlatformType";
    public const string LaunchIsPlatformTypeEnabled = "Launch.IsPlatformTypeEnabled";
    public const string LaunchIsWindowsHDREnabled = "Launch.IsWindowsHDREnabled";
    public const string LaunchUsingStarwardPlayTimeStatistics = "Launch.UseStarwardPlayTimeStatistics";
    public const string LaunchUsingBetterGenshinImpactAutomation = "Launch.UseBetterGenshinImpactAutomation";
    public const string LaunchSetDiscordActivityWhenPlaying = "Launch.SetDiscordActivityWhenPlaying";
    public const string LaunchUsingHoyolabAccount = "Launch.UsingHoyolabAccount";
    public const string LaunchUsingOverlay = "Launch.UsingOverlay";

    [Obsolete("不再区分解锁器类型，统一使用注入")]
    public const string LaunchUnlockerKind = "Launch.UnlockerKind";

    [Obsolete("不再支持多开")]
    public const string MultipleInstances = "Launch.MultipleInstances";

    [Obsolete("不再使用 PowerShell")]
    public const string PowerShellPath = "PowerShellPath";

    [Obsolete("不再要求智力测试")]
    public const string IsAdvancedLaunchOptionsEnabled = "IsAdvancedLaunchOptionsEnabled";

    [Obsolete("不再使用此名称")]
    public const string LaunchLoopAdjustFpsOnly = "Launch.LoopAdjustFpsOnly";

    [Obsolete("不再直接使用 CLOUD_THIRD_PARTY_MOBILE")]
    public const string LaunchUsingCloudThirdPartyMobile = "Launch.IsUseCloudThirdPartyMobile";

    [Obsolete("不再使用")]
    public const string LaunchHookingSetFieldOfView = "Launch.HookingSetFieldOfView";

    [Obsolete("不再使用")]
    public const string LaunchHookingOpenTeam = "Launch.HookingOpenTeam";

    [Obsolete("不再使用")]
    public const string LaunchHookingMickyWonderPartner2 = "Launch.HookingMickyWonderPartner2";

    [Obsolete("不再使用")]
    public const string LaunchHookingSetupQuestBanner = "Launch.HookingSetupQuestBanner";

    [Obsolete("不再使用")]
    public const string LaunchHookingEventCameraMove = "Launch.HookingEventCameraMove";

    [Obsolete("不再使用")]
    public const string LaunchHookingShowDamageText = "Launch.HookingShowDamageText";

    [Obsolete("不再使用")]
    public const string IsNotifyIconEnabled = "IsNotifyIconEnabled";

    [Obsolete("不再使用")]
    public const string PackageConverterType = "PackageConverterType";
}