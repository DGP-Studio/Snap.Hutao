﻿// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Snap.Hutao.Model;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Abstraction;
using Snap.Hutao.Service.BackgroundImage;
using Snap.Hutao.UI.Xaml.Media.Backdrop;
using Snap.Hutao.Web.Hoyolab;

namespace Snap.Hutao.Service;

[ConstructorGenerated(CallBaseConstructor = true)]
[Injection(InjectAs.Singleton)]
internal sealed partial class AppOptions : DbStoreOptions
{
    private bool? isNotifyIconEnabled;
    private bool? isEmptyHistoryWishVisible;
    private bool? isUnobtainedWishItemVisible;
    private BackdropType? backdropType;
    private ElementTheme? elementTheme;
    private BackgroundImageType? backgroundImageType;
    private Region? region;
    private string? geetestCustomCompositeUrl;

    public bool IsNotifyIconEnabled
    {
        get => GetOption(ref isNotifyIconEnabled, SettingEntry.IsNotifyIconEnabled, true);
        set => SetOption(ref isNotifyIconEnabled, SettingEntry.IsNotifyIconEnabled, value);
    }

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

    public List<NameValue<BackdropType>> BackdropTypes { get; } = CollectionsNameValue.FromEnum<BackdropType>(type => type >= 0);

    public BackdropType BackdropType
    {
        get => GetOption(ref backdropType, SettingEntry.SystemBackdropType, EnumParse<BackdropType>, BackdropType.Mica).Value;
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
        get => GetOption(ref elementTheme, SettingEntry.ElementTheme, EnumParse<ElementTheme>, ElementTheme.Default).Value;
        set => SetOption(ref elementTheme, SettingEntry.ElementTheme, value, EnumToStringOrEmpty);
    }

    public List<NameValue<BackgroundImageType>> BackgroundImageTypes { get; } = CollectionsNameValue.FromEnum<BackgroundImageType>(type => type.GetLocalizedDescription());

    public BackgroundImageType BackgroundImageType
    {
        get => GetOption(ref backgroundImageType, SettingEntry.BackgroundImageType, EnumParse<BackgroundImageType>, BackgroundImageType.None).Value;
        set => SetOption(ref backgroundImageType, SettingEntry.BackgroundImageType, value, EnumToStringOrEmpty);
    }

    public Lazy<List<NameValue<Region>>> LazyRegions { get; } = new(KnownRegions.Get);

    public Region Region
    {
        get => GetOption(ref region, SettingEntry.AnnouncementRegion, v => Region.FromRegionString(v), Region.CNGF01).Value;
        set => SetOption(ref region, SettingEntry.AnnouncementRegion, value, v => v.ToStringOrEmpty());
    }

    public string GeetestCustomCompositeUrl
    {
        get => GetOption(ref geetestCustomCompositeUrl, SettingEntry.GeetestCustomCompositeUrl);
        set => SetOption(ref geetestCustomCompositeUrl, SettingEntry.GeetestCustomCompositeUrl, value);
    }
}