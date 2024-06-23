// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;

namespace Snap.Hutao.UI.Xaml.Control.Theme;

/// <summary>
/// 主题帮助工具类
/// </summary>
[HighQuality]
internal static class ThemeHelper
{
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