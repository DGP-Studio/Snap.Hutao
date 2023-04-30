// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Abstraction;
using System.Globalization;

namespace Snap.Hutao.Service;

/// <summary>
/// 应用程序选项
/// 存储服务相关的选项
/// </summary>
[Injection(InjectAs.Singleton)]
internal sealed class AppOptions : DbStoreOptions
{
    private string? gamePath;
    private bool? isEmptyHistoryWishVisible;
    private Core.Windowing.BackdropType? backdropType;
    private CultureInfo? currentCulture;
    private bool? isAdvancedLaunchOptionsEnabled;

    /// <summary>
    /// 构造一个新的应用程序选项
    /// </summary>
    /// <param name="serviceScopeFactory">服务范围工厂</param>
    public AppOptions(IServiceScopeFactory serviceScopeFactory)
        : base(serviceScopeFactory)
    {
    }

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
    /// 背景类型 默认 Mica
    /// </summary>
    public Core.Windowing.BackdropType BackdropType
    {
        get => GetOption(ref backdropType, SettingEntry.SystemBackdropType, Enum.Parse<Core.Windowing.BackdropType>, Core.Windowing.BackdropType.Mica);
        set => SetOption(ref backdropType, SettingEntry.SystemBackdropType, value, value => value.ToString());
    }

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
}