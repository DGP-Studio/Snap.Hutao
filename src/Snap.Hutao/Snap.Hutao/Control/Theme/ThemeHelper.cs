// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml;

namespace Snap.Hutao.Control.Theme;

/// <summary>
/// 主题帮助工具类
/// </summary>
[HighQuality]
internal static class ThemeHelper
{
    /// <summary>
    /// 判断主题是否相等
    /// </summary>
    /// <param name="applicationTheme">应用主题</param>
    /// <param name="elementTheme">元素主题</param>
    /// <returns>主题是否相等</returns>
    public static bool Equals(ApplicationTheme applicationTheme, ElementTheme elementTheme)
    {
        return (applicationTheme, elementTheme) switch
        {
            (ApplicationTheme.Light, ElementTheme.Light) => true,
            (ApplicationTheme.Dark, ElementTheme.Dark) => true,
            _ => false,
        };
    }

    /// <summary>
    /// 从 <see cref="ApplicationTheme"/> 转换到 <see cref="ElementTheme"/>
    /// </summary>
    /// <param name="applicationTheme">应用主题</param>
    /// <returns>元素主题</returns>
    public static ElementTheme ApplicationToElement(ApplicationTheme applicationTheme)
    {
        return applicationTheme switch
        {
            ApplicationTheme.Light => ElementTheme.Light,
            ApplicationTheme.Dark => ElementTheme.Dark,
            _ => throw Must.NeverHappen(),
        };
    }

    /// <summary>
    /// 从 <see cref="ElementTheme"/> 转换到 <see cref="ApplicationTheme"/>
    /// </summary>
    /// <param name="applicationTheme">元素主题</param>
    /// <returns>应用主题</returns>
    public static ApplicationTheme ElementToApplication(ElementTheme applicationTheme)
    {
        return applicationTheme switch
        {
            ElementTheme.Light => ApplicationTheme.Light,
            ElementTheme.Dark => ApplicationTheme.Dark,
            _ => Ioc.Default.GetRequiredService<App>().RequestedTheme,
        };
    }

    /// <summary>
    /// 从 <see cref="ElementTheme"/> 转换到 <see cref="SystemBackdropTheme"/>
    /// </summary>
    /// <param name="elementTheme">元素主题</param>
    /// <returns>背景主题</returns>
    public static SystemBackdropTheme ElementToSystemBackdrop(ElementTheme elementTheme)
    {
        return elementTheme switch
        {
            ElementTheme.Default => SystemBackdropTheme.Default,
            ElementTheme.Light => SystemBackdropTheme.Light,
            ElementTheme.Dark => SystemBackdropTheme.Dark,
            _ => throw Must.NeverHappen(),
        };
    }

    /// <summary>
    /// 检查是否为暗黑模式
    /// </summary>
    /// <param name="elementTheme">当前元素主题</param>
    /// <returns>是否为暗黑模式</returns>
    public static bool IsDarkMode(ElementTheme elementTheme)
    {
        ApplicationTheme appTheme = Ioc.Default.GetRequiredService<App>().RequestedTheme;
        return IsDarkMode(elementTheme, appTheme);
    }

    /// <summary>
    /// 检查是否为暗黑模式
    /// </summary>
    /// <param name="elementTheme">当前元素主题</param>
    /// <param name="applicationTheme">当前应用主题</param>
    /// <returns>是否为暗黑模式</returns>
    public static bool IsDarkMode(ElementTheme elementTheme, ApplicationTheme applicationTheme)
    {
        return elementTheme switch
        {
            ElementTheme.Default => applicationTheme == ApplicationTheme.Dark,
            ElementTheme.Dark => true,
            _ => false,
        };
    }
}