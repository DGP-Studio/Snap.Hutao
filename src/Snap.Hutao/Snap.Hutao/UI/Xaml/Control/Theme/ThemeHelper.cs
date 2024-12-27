// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Snap.Hutao.Core.ExceptionService;

namespace Snap.Hutao.UI.Xaml.Control.Theme;

internal static class ThemeHelper
{
    public static Snap.WinUI.FrameworkTheming.Theme ElementToFramework(ElementTheme elementTheme)
    {
        return elementTheme switch
        {
            ElementTheme.Default => WinUI.FrameworkTheming.Theme.None,
            ElementTheme.Light => WinUI.FrameworkTheming.Theme.Light,
            ElementTheme.Dark => WinUI.FrameworkTheming.Theme.Dark,
            _ => throw HutaoException.NotSupported($"Unexpected ElementTheme value: {elementTheme}."),
        };
    }

    public static Snap.WinUI.FrameworkTheming.Theme ApplicationToFramework(ApplicationTheme applicationTheme)
    {
        return applicationTheme switch
        {
            ApplicationTheme.Light => WinUI.FrameworkTheming.Theme.Light,
            ApplicationTheme.Dark => WinUI.FrameworkTheming.Theme.Dark,
            _ => throw HutaoException.NotSupported($"Unexpected ElementTheme value: {applicationTheme}."),
        };
    }

    public static Snap.WinUI.FrameworkTheming.Theme ApplicationToFrameworkInvert(ApplicationTheme applicationTheme)
    {
        return applicationTheme switch
        {
            ApplicationTheme.Light => WinUI.FrameworkTheming.Theme.Dark,
            ApplicationTheme.Dark => WinUI.FrameworkTheming.Theme.Light,
            _ => throw HutaoException.NotSupported($"Unexpected ElementTheme value: {applicationTheme}."),
        };
    }

    public static ApplicationTheme ElementToApplication(ElementTheme applicationTheme)
    {
        return applicationTheme switch
        {
            ElementTheme.Light => ApplicationTheme.Light,
            ElementTheme.Dark => ApplicationTheme.Dark,
            _ => Ioc.Default.GetRequiredService<App>().RequestedTheme,
        };
    }

    public static ElementTheme ApplicationToElement(ApplicationTheme applicationTheme)
    {
        return applicationTheme switch
        {
            ApplicationTheme.Light => ElementTheme.Light,
            ApplicationTheme.Dark => ElementTheme.Dark,
            _ => ElementTheme.Default,
        };
    }

    public static bool IsDarkMode(ApplicationTheme applicationTheme)
    {
        return applicationTheme is ApplicationTheme.Dark;
    }

    public static bool IsDarkMode(ElementTheme elementTheme)
    {
        ApplicationTheme appTheme = Ioc.Default.GetRequiredService<App>().RequestedTheme;
        return IsDarkMode(elementTheme, appTheme);
    }

    public static bool IsDarkMode(ElementTheme elementTheme, ApplicationTheme applicationTheme)
    {
        return elementTheme switch
        {
            ElementTheme.Default => IsDarkMode(applicationTheme),
            ElementTheme.Dark => true,
            _ => false,
        };
    }
}