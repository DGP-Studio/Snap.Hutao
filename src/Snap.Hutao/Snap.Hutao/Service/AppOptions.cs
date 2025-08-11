// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Snap.Hutao.Model;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Abstraction;
using Snap.Hutao.Service.BackgroundImage;
using Snap.Hutao.UI.Xaml.Media.Backdrop;
using Snap.Hutao.Web.Bridge;
using Snap.Hutao.Web.Hoyolab;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Globalization;

namespace Snap.Hutao.Service;

[ConstructorGenerated(CallBaseConstructor = true)]
[Service(ServiceLifetime.Singleton)]
internal sealed partial class AppOptions : DbStoreOptions
{
    private bool? isEmptyHistoryWishVisible;
    private bool? isUnobtainedWishItemVisible;
    private BackdropType? backdropType;
    private ElementTheme? elementTheme;
    private BackgroundImageType? backgroundImageType;
    private Region? region;
    private int? downloadSpeedLimitPerSecondInKiloByte;
    private BridgeShareSaveType? bridgeShareSaveType;
    private TimeSpan? calendarServerTimeZoneOffset;
    private LastWindowCloseBehavior? lastWindowCloseBehavior;

    public static bool NotifyIconCreated { get => XamlApplicationLifetime.NotifyIconCreated; }

    public bool IsEmptyHistoryWishVisible
    {
        get => GetOption(ref isEmptyHistoryWishVisible, SettingEntry.IsEmptyHistoryWishVisible, false);
        set => SetOption(ref isEmptyHistoryWishVisible, SettingEntry.IsEmptyHistoryWishVisible, value);
    }

    public bool IsUnobtainedWishItemVisible
    {
        get => GetOption(ref isUnobtainedWishItemVisible, SettingEntry.IsUnobtainedWishItemVisible, false);
        set => SetOption(ref isUnobtainedWishItemVisible, SettingEntry.IsUnobtainedWishItemVisible, value);
    }

    public ImmutableArray<NameValue<BackdropType>> BackdropTypes { get; } = ImmutableCollectionsNameValue.FromEnum<BackdropType>(type => type >= 0);

    public BackdropType BackdropType
    {
        get => GetOption(ref backdropType, SettingEntry.SystemBackdropType, Enum.Parse<BackdropType>, BackdropType.Mica);
        set => SetOption(ref backdropType, SettingEntry.SystemBackdropType, value, EnumToStringOrEmpty);
    }

    public Lazy<List<NameValue<ElementTheme>>> LazyElementThemes { get; } = new(() =>
    [
        new(SH.CoreWindowThemeLight, ElementTheme.Light),
        new(SH.CoreWindowThemeDark, ElementTheme.Dark),
        new(SH.CoreWindowThemeSystem, ElementTheme.Default),
    ]);

    public ElementTheme ElementTheme
    {
        get => GetOption(ref elementTheme, SettingEntry.ElementTheme, Enum.Parse<ElementTheme>, ElementTheme.Default);
        set => SetOption(ref elementTheme, SettingEntry.ElementTheme, value, EnumToStringOrEmpty);
    }

    public ImmutableArray<NameValue<BackgroundImageType>> BackgroundImageTypes { get; } = ImmutableCollectionsNameValue.FromEnum<BackgroundImageType>(type => type.GetLocalizedDescription(SH.ResourceManager, CultureInfo.CurrentCulture));

    public BackgroundImageType BackgroundImageType
    {
        get => GetOption(ref backgroundImageType, SettingEntry.BackgroundImageType, Enum.Parse<BackgroundImageType>, BackgroundImageType.None);
        set => SetOption(ref backgroundImageType, SettingEntry.BackgroundImageType, value, EnumToStringOrEmpty);
    }

    public Lazy<ImmutableArray<NameValue<Region>>> LazyRegions { get; } = new(() =>
    {
        Debug.Assert(XamlApplicationLifetime.CultureInfoInitialized);
        return KnownRegions.Value;
    });

    public Region Region
    {
        get => GetOption(ref region, SettingEntry.AnnouncementRegion, Region.FromRegionString, Region.CNGF01);
        set => SetOption(ref region, SettingEntry.AnnouncementRegion, value, NullableExtension.ToStringOrEmpty);
    }

    [field: AllowNull]
    public string GeetestCustomCompositeUrl
    {
        get => GetOption(ref field, SettingEntry.GeetestCustomCompositeUrl);
        set => SetOption(ref field, SettingEntry.GeetestCustomCompositeUrl, value);
    }

    public int DownloadSpeedLimitPerSecondInKiloByte
    {
        get => GetOption(ref downloadSpeedLimitPerSecondInKiloByte, SettingEntry.DownloadSpeedLimitPerSecondInKiloByte, 0);
        set => SetOption(ref downloadSpeedLimitPerSecondInKiloByte, SettingEntry.DownloadSpeedLimitPerSecondInKiloByte, value);
    }

    public ImmutableArray<NameValue<BridgeShareSaveType>> BridgeShareSaveTypes { get; } = ImmutableCollectionsNameValue.FromEnum<BridgeShareSaveType>(type => type.GetLocalizedDescription(SH.ResourceManager, CultureInfo.CurrentCulture));

    public BridgeShareSaveType BridgeShareSaveType
    {
        get => GetOption(ref bridgeShareSaveType, SettingEntry.BridgeShareSaveType, Enum.Parse<BridgeShareSaveType>, BridgeShareSaveType.CopyToClipboard);
        set => SetOption(ref bridgeShareSaveType, SettingEntry.BridgeShareSaveType, value, EnumToStringOrEmpty);
    }

    public Lazy<ImmutableArray<NameValue<TimeSpan>>> LazyCalendarServerTimeZoneOffsets { get; } = new(() =>
    {
        Debug.Assert(XamlApplicationLifetime.CultureInfoInitialized);
        return KnownServerRegionTimeZones.Value;
    });

    public TimeSpan CalendarServerTimeZoneOffset
    {
        get => GetOption(ref calendarServerTimeZoneOffset, SettingEntry.CalendarServerTimeZoneOffset, TimeSpan.Parse, ServerRegionTimeZone.CommonOffset);
        set => SetOption(ref calendarServerTimeZoneOffset, SettingEntry.CalendarServerTimeZoneOffset, value, static v => v.ToString());
    }

    public ImmutableArray<NameValue<LastWindowCloseBehavior>> LastWindowCloseBehaviors { get; } = ImmutableCollectionsNameValue.FromEnum<LastWindowCloseBehavior>(static @enum => @enum.GetLocalizedDescription(SH.ResourceManager, CultureInfo.CurrentCulture));

    public LastWindowCloseBehavior LastWindowCloseBehavior
    {
        get => GetOption(ref lastWindowCloseBehavior, SettingEntry.LastWindowCloseBehavior, Enum.Parse<LastWindowCloseBehavior>, LastWindowCloseBehavior.EnsureNotifyIconCreated);
        set => SetOption(ref lastWindowCloseBehavior, SettingEntry.LastWindowCloseBehavior, value, EnumToStringOrEmpty);
    }
}