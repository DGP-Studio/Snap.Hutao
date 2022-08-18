// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml;

namespace Snap.Hutao.Core;

/// <summary>
/// 主题帮助工具类
/// </summary>
public static class ThemeHelper
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
}