// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Windowing;
using Snap.Hutao.Model;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Abstraction;
using System.Globalization;

namespace Snap.Hutao.Service;

/// <summary>
/// 应用程序选项
/// 存储服务相关的选项
/// </summary>
[ConstructorGenerated(CallBaseConstructor = true)]
[Injection(InjectAs.Singleton)]
internal sealed partial class AppOptions : DbStoreOptions
{
    private readonly List<NameValue<BackdropType>> supportedBackdropTypesInner = new()
    {
        new("Acrylic", BackdropType.Acrylic),
        new("Mica", BackdropType.Mica),
        new("MicaAlt", BackdropType.MicaAlt),
    };

    private readonly List<NameValue<string>> supportedCulturesInner = new()
    {
        ToNameValue(CultureInfo.GetCultureInfo("zh-Hans")),
        ToNameValue(CultureInfo.GetCultureInfo("zh-Hant")),
        ToNameValue(CultureInfo.GetCultureInfo("en")),
        ToNameValue(CultureInfo.GetCultureInfo("ko")),
        ToNameValue(CultureInfo.GetCultureInfo("ja")),
    };

    private string? gamePath;
    private bool? isEmptyHistoryWishVisible;
    private BackdropType? backdropType;
    private CultureInfo? currentCulture;
    private bool? isAdvancedLaunchOptionsEnabled;
    private string? geetestCustomCompositeUrl;

    /// <summary>
    /// 游戏路径
    /// </summary>
    public string GamePath
    {
        get => GetOption(ref gamePath, SettingEntry.GamePath);
        set => SetOption(ref gamePath, SettingEntry.GamePath, value);
    }

    /// <summary>
    /// 游戏路径
    /// </summary>
    public bool IsEmptyHistoryWishVisible
    {
        get => GetOption(ref isEmptyHistoryWishVisible, SettingEntry.IsEmptyHistoryWishVisible);
        set => SetOption(ref isEmptyHistoryWishVisible, SettingEntry.IsEmptyHistoryWishVisible, value);
    }

    /// <summary>
    /// 所有支持的背景样式
    /// </summary>
    public List<NameValue<BackdropType>> BackdropTypes { get => supportedBackdropTypesInner; }

    /// <summary>
    /// 背景类型 默认 Mica
    /// </summary>
    public BackdropType BackdropType
    {
        get => GetOption(ref backdropType, SettingEntry.SystemBackdropType, Enum.Parse<BackdropType>, BackdropType.Mica);
        set => SetOption(ref backdropType, SettingEntry.SystemBackdropType, value, value => value.ToString());
    }

    /// <summary>
    /// 所有支持的语言
    /// </summary>
    public List<NameValue<string>> Cultures { get => supportedCulturesInner; }

    /// <summary>
    /// 初始化前的语言
    /// 通过设置与获取此属性，就可以获取到与系统同步的语言
    /// </summary>
    public CultureInfo PreviousCulture { get; set; } = default!;

    /// <summary>
    /// 当前语言
    /// </summary>
    public CultureInfo CurrentCulture
    {
        get => GetOption(ref currentCulture, SettingEntry.Culture, CultureInfo.GetCultureInfo, CultureInfo.CurrentCulture);
        set => SetOption(ref currentCulture, SettingEntry.Culture, value, value => value.Name);
    }

    /// <summary>
    /// 是否启用高级功能
    /// </summary>
    public bool IsAdvancedLaunchOptionsEnabled
    {
        get => GetOption(ref isAdvancedLaunchOptionsEnabled, SettingEntry.IsAdvancedLaunchOptionsEnabled);
        set => SetOption(ref isAdvancedLaunchOptionsEnabled, SettingEntry.IsAdvancedLaunchOptionsEnabled, value);
    }

    public string GeetestCustomCompositeUrl
    {
        get => GetOption(ref geetestCustomCompositeUrl, SettingEntry.GeetestCustomCompositeUrl);
        set => SetOption(ref geetestCustomCompositeUrl, SettingEntry.GeetestCustomCompositeUrl, value);
    }

    private static NameValue<string> ToNameValue(CultureInfo info)
    {
        return new(info.NativeName, info.Name);
    }
}