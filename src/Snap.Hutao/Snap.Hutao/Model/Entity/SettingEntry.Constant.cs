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
    public const string Culture = "Culture";

    public const string IsNotifyIconEnabled = "IsNotifyIconEnabled";
    public const string SystemBackdropType = "SystemBackdropType";
    public const string ElementTheme = "ElementTheme";
    public const string BackgroundImageType = "BackgroundImageType";

    public const string AnnouncementRegion = "AnnouncementRegion";

    public const string IsEmptyHistoryWishVisible = "IsEmptyHistoryWishVisible";
    public const string IsUnobtainedWishItemVisible = "IsUnobtainedWishItemVisible";

    public const string GeetestCustomCompositeUrl = "GeetestCustomCompositeUrl";

    public const string DailyNoteIsAutoRefreshEnabled = "DailyNote.IsAutoRefreshEnabled";
    public const string DailyNoteRefreshSeconds = "DailyNote.RefreshSeconds";
    public const string DailyNoteReminderNotify = "DailyNote.ReminderNotify";
    public const string DailyNoteSilentWhenPlayingGame = "DailyNote.SilentWhenPlayingGame";
    public const string DailyNoteWebhookUrl = "DailyNote.WebhookUrl";

    public const string IsAdvancedLaunchOptionsEnabled = "IsAdvancedLaunchOptionsEnabled";

    public const string LaunchIsLaunchOptionsEnabled = "Launch.IsLaunchOptionsEnabled";
    public const string LaunchIsExclusive = "Launch.IsExclusive";
    public const string LaunchIsFullScreen = "Launch.IsFullScreen";
    public const string LaunchIsBorderless = "Launch.IsBorderless";
    public const string LaunchScreenWidth = "Launch.ScreenWidth";
    public const string LaunchIsScreenWidthEnabled = "Launch.IsScreenWidthEnabled";
    public const string LaunchScreenHeight = "Launch.ScreenHeight";
    public const string LaunchIsScreenHeightEnabled = "Launch.IsScreenHeightEnabled";
    public const string LaunchUnlockFps = "Launch.UnlockFps";
    public const string LaunchTargetFps = "Launch.TargetFps";
    public const string LaunchTargetFov = "Launch.TargetFov";
    public const string LaunchDisableFog = "Launch.DisableFog";
    public const string LaunchMonitor = "Launch.Monitor";
    public const string LaunchIsMonitorEnabled = "Launch.IsMonitorEnabled";
    public const string LaunchIsUseCloudThirdPartyMobile = "Launch.IsUseCloudThirdPartyMobile";
    public const string LaunchIsWindowsHDREnabled = "Launch.IsWindowsHDREnabled";
    public const string LaunchUseStarwardPlayTimeStatistics = "Launch.UseStarwardPlayTimeStatistics";
    public const string LaunchUseBetterGenshinImpactAutomation = "Launch.UseBetterGenshinImpactAutomation";
    public const string LaunchSetDiscordActivityWhenPlaying = "Launch.SetDiscordActivityWhenPlaying";

    public const string PackageIsPredownloadFinished = "Package.IsPredownloadFinished";

    [Obsolete("不再区分解锁器类型，统一使用注入")]
    public const string LaunchUnlockerKind = "Launch.UnlockerKind";

    [Obsolete("不再支持多开")]
    public const string MultipleInstances = "Launch.MultipleInstances";

    [Obsolete("不再使用 PowerShell")]
    public const string PowerShellPath = "PowerShellPath";
}
