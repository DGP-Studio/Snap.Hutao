// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Snap.Hutao.Core.Windowing;
using Snap.Hutao.Model;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Abstraction;
using Snap.Hutao.Service.BackgroundImage;
using Snap.Hutao.Web.Hoyolab;

namespace Snap.Hutao.Service;

[ConstructorGenerated(CallBaseConstructor = true)]
[Injection(InjectAs.Singleton)]
internal sealed partial class AppOptions : DbStoreOptions
{
    private bool? isNeverHeldStatisticsItemVisible;
    private bool? isEmptyHistoryWishVisible;
    private BackdropType? backdropType;
    private ElementTheme? elementTheme;
    private BackgroundImageType? backgroundImageType;
    private Region? region;
    private string? geetestCustomCompositeUrl;

    public bool IsEmptyHistoryWishVisible
    {
        get => GetOption(ref isEmptyHistoryWishVisible, SettingEntry.IsEmptyHistoryWishVisible);
        set => SetOption(ref isEmptyHistoryWishVisible, SettingEntry.IsEmptyHistoryWishVisible, value);
    }

    public bool IsNeverHeldStatisticsItemVisible
    {
        get => GetOption(ref isNeverHeldStatisticsItemVisible, SettingEntry.IsNeverHeldStatisticsItemVisible);
        set => SetOption(ref isNeverHeldStatisticsItemVisible, SettingEntry.IsNeverHeldStatisticsItemVisible, value);
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
        set => SetOption(ref region, SettingEntry.AnnouncementRegion, value, value => value.ToStringOrEmpty());
    }

    public string GeetestCustomCompositeUrl
    {
        get => GetOption(ref geetestCustomCompositeUrl, SettingEntry.GeetestCustomCompositeUrl);
        set => SetOption(ref geetestCustomCompositeUrl, SettingEntry.GeetestCustomCompositeUrl, value);
    }

    private static T? EnumParse<T>(string input)
        where T : struct, Enum
    {
        return Enum.Parse<T>(input);
    }

    private static string EnumToStringOrEmpty<T>(T? input)
        where T : struct, Enum
    {
        return input.ToStringOrEmpty();
    }
}