// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml;
using Snap.Hutao.Core.ExceptionService;

namespace Snap.Hutao.UI.Xaml.Control.Theme;

internal static class ThemeHelper
{
    public static SystemBackdropTheme ElementToSystemBackdrop(ElementTheme elementTheme)
    {
        return elementTheme switch
        {
            ElementTheme.Default => SystemBackdropTheme.Default,
            ElementTheme.Light => SystemBackdropTheme.Light,
            ElementTheme.Dark => SystemBackdropTheme.Dark,
            _ => throw HutaoException.NotSupported($"Unexpected ElementTheme value: {elementTheme}."),
        };
    }

    public static Snap.Hutao.UI.Xaml.Theme ElementToFramework(ElementTheme elementTheme)
    {
        return elementTheme switch
        {
            ElementTheme.Default => Snap.Hutao.UI.Xaml.Theme.None,
            ElementTheme.Light => Snap.Hutao.UI.Xaml.Theme.Light,
            ElementTheme.Dark => Snap.Hutao.UI.Xaml.Theme.Dark,
            _ => throw HutaoException.NotSupported($"Unexpected ElementTheme value: {elementTheme}."),
        };
    }

    public static Snap.Hutao.UI.Xaml.Theme ApplicationToFramework(ApplicationTheme applicationTheme)
    {
        return applicationTheme switch
        {
            ApplicationTheme.Light => Snap.Hutao.UI.Xaml.Theme.Light,
            ApplicationTheme.Dark => Snap.Hutao.UI.Xaml.Theme.Dark,
            _ => throw HutaoException.NotSupported($"Unexpected ApplicationTheme value: {applicationTheme}."),
        };
    }

    public static Snap.Hutao.UI.Xaml.Theme ApplicationToFrameworkInvert(ApplicationTheme applicationTheme)
    {
        return applicationTheme switch
        {
            ApplicationTheme.Light => Snap.Hutao.UI.Xaml.Theme.Dark,
            ApplicationTheme.Dark => Snap.Hutao.UI.Xaml.Theme.Light,
            _ => throw HutaoException.NotSupported($"Unexpected ApplicationTheme value: {applicationTheme}."),
        };
    }

    public static ApplicationTheme ElementToApplication(ElementTheme applicationTheme)
    {
        return applicationTheme switch
        {
            ElementTheme.Light => ApplicationTheme.Light,
            ElementTheme.Dark => ApplicationTheme.Dark,
            _ => Application.Current.RequestedTheme,
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
        ApplicationTheme appTheme = Application.Current.RequestedTheme;
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